using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace SignRequest
{
    public class SignRequestService
    {
        public HttpRequestMessage GetSignedRequest(RequestParams reqParams)
        {
            var signature = GetSignature(reqParams);
            var qs = GetRequestQueryString(reqParams, signature);
            var request = new HttpRequestMessage(HttpMethod.Get, $"?{qs}");
            if (reqParams.useAuthorizationHeader)
            {
                var authHeader = $"{reqParams.algorithm} Credential={reqParams.accessKeyId}/{reqParams.credentialScope}, SignedHeaders={reqParams.signedHeaders}, Signature={signature}";
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            }

            request.Headers.Add("Host", "iam.amazonaws.com");
            request.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            request.Headers.Add("x-amz-date", reqParams.amzdate);
            return request;
        }
        private string GetSignature(RequestParams par)
        {
            var canonicalQs = $"Action={par.action}&Version={par.version}";
            if (!par.useAuthorizationHeader)
            {
                var securityToken = !string.IsNullOrEmpty(par.encodedSecurityToken) ? "&X-Amz-Security-Token=" + par.encodedSecurityToken : "";
                canonicalQs += $"&X-Amz-Algorithm={par.algorithm}&X-Amz-Credential={par.encodedCredentials}&X-Amz-Date={par.amzdate}&X-Amz-Expires=60{securityToken}&X-Amz-SignedHeaders={par.encodedSignedHeaders}";
            }

            var canonicalRequest = GetCanonicalRequest("GET", "/", canonicalQs, par.headers, par.signedHeaders, par.payload);
            var canonicalRequestHash = GetHexString(Hash(canonicalRequest));

            // calculate string to sign
            var stringToSign = new StringBuilder();
            stringToSign.Append(par.algorithm + "\n");
            stringToSign.Append(par.amzdate + "\n");
            stringToSign.Append(par.credentialScope + "\n");
            stringToSign.Append(canonicalRequestHash);

            // calculate signature
            var signatureKey = getSignatureKey(par.secret, par.date, par.region, par.service);
            var signature = GetHexString(HmacSHA256(stringToSign.ToString(), signatureKey));
            return signature;
        }

        private string GetRequestQueryString(RequestParams par, string signature)
        {
            var qs = new StringBuilder();
            qs.Append($"Action={par.action}");
            qs.Append($"&Version={par.version}");
            if (!par.useAuthorizationHeader)
            {
                qs.Append($"&X-Amz-Algorithm={par.algorithm}");
                qs.Append($"&X-Amz-Credential={par.encodedCredentials}");
                qs.Append($"&X-Amz-Date={par.amzdate}");
                qs.Append("&X-Amz-Expires=60");
                if (!string.IsNullOrEmpty(par.encodedSecurityToken))
                {
                    qs.Append($"&X-Amz-Security-Token={par.encodedSecurityToken}");
                }
                qs.Append($"&X-Amz-SignedHeaders={par.encodedSignedHeaders}");
                qs.Append($"&X-Amz-Signature={signature}");
            }
            return qs.ToString();
        }

        private string GetCanonicalRequest(string method, string uri, string queryString, Dictionary<string, string> headers, string signedHeaders, string payload)
        {
            var builder = new StringBuilder();
            builder.Append(method + "\n");
            builder.Append(uri + "\n");
            builder.Append(queryString + "\n");
            foreach (KeyValuePair<string, string> entry in headers)
            {
                builder.Append($"{entry.Key.ToLower()}:{entry.Value.Trim()}\n");
            }
            builder.Append("\n");
            builder.Append(signedHeaders + "\n");

            var hashedPayload = GetHexString(Hash(payload));
            builder.Append(hashedPayload);
            return builder.ToString();
        }

        private byte[] HmacSHA256(string data, byte[] key)
        {
            var algorithm = "HmacSHA256";
            var kha = KeyedHashAlgorithm.Create(algorithm);
            kha.Key = key;

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private byte[] Hash(string data)
        {
            var algorithm = "SHA256";
            var kha = HashAlgorithm.Create(algorithm);

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private string GetHexString(byte[] val)
        {
            var hexString = BitConverter.ToString(val);
            hexString = hexString.Replace("-", "");
            return hexString.ToLower();
        }

        private byte[] getSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes(("AWS4" + key).ToCharArray());
            byte[] kDate = HmacSHA256(dateStamp, kSecret);
            byte[] kRegion = HmacSHA256(regionName, kDate);
            byte[] kService = HmacSHA256(serviceName, kRegion);
            byte[] kSigning = HmacSHA256("aws4_request", kService);

            return kSigning;
        }
    }
}
