using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SignRequest
{
    class RequestParams
    {
        public string secret { get; set; }
        public string accessKeyId { get; set; }
        public string region { get; set; }
        public string service { get; set; }
        public string action { get; set; }
        public string algorithm { get; set; }
        public string amzdate { get; set; }
        public string date { get; set; }
        public string version { get; set; }
        public string payload { get; set; }
        public bool useAuthorizationHeader { get; set; }
        public string encodedCredentials { 
            get { 
               return WebUtility.UrlEncode($"{accessKeyId}/{credentialScope}");
            } 
        }
        public string encodedSignedHeaders
        {
            get
            {
                return WebUtility.UrlEncode(signedHeaders);
            }
        }
        public Dictionary<string, string> headers { get; set; }
        public string credentialScope { 
            get {
                return $"{date}/{region}/{service}/aws4_request"; 
           } 
        }
        public string signedHeaders
        {
            get
            {
                return string.Join(';', headers.Keys.Select(k => k));
            }
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            var currDate = DateTime.Now.ToUniversalTime();
            var amzdate = $"{currDate:yyyyMMdd}T{currDate:HHmmss}Z";
            var requestParams = new RequestParams
            {
                secret = "D/xf1mx+suNaU+y9ZYrTuX2j/b/aN53hdsoIks3Z",
                accessKeyId = "AKIAU64D5FG2J5P5MZ47",
                region = "us-east-1",
                service = "iam",
                action = "ListUsers",
                algorithm = "AWS4-HMAC-SHA256",
                amzdate = amzdate,
                date = currDate.ToString("yyyyMMdd"),
                version = "2010-05-08",
                payload = "",
                useAuthorizationHeader = false,
                headers = new Dictionary<string, string>
                {
                    //{ "content-type", "application/x-www-form-urlencoded; charset=utf-8" },
                    { "host", "iam.amazonaws.com" },
                    { "x-amz-date", amzdate }
                }
            };

            var signature = GetSignature(requestParams);

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://iam.amazonaws.com");
            var qs = GetRequestQueryString(requestParams, signature);
            var request = new HttpRequestMessage(HttpMethod.Get, $"?{qs}");
            if (requestParams.useAuthorizationHeader)
            {
                var authHeader = $"{requestParams.algorithm} Credential={requestParams.accessKeyId}/{requestParams.credentialScope}, SignedHeaders={requestParams.signedHeaders}, Signature={signature}";
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            }
     
            request.Headers.Add("Host", "iam.amazonaws.com");
            request.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            request.Headers.Add("x-amz-date", amzdate);
            
            var response = await client.SendAsync(request);
            var responseMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseMessage);
            Console.ReadLine();
        }

        private static string GetSignature(RequestParams par)
        {
            var canonicalQs = $"Action={par.action}&Version={par.version}";
            if (!par.useAuthorizationHeader)
            {
                canonicalQs += $"&X-Amz-Algorithm={par.algorithm}&X-Amz-Credential={par.encodedCredentials}&X-Amz-Date={par.amzdate}&X-Amz-Expires=60&X-Amz-SignedHeaders={par.encodedSignedHeaders}";
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

        private static string GetRequestQueryString(RequestParams par, string signature)
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
                qs.Append($"&X-Amz-SignedHeaders={par.encodedSignedHeaders}");
                qs.Append($"&X-Amz-Signature={signature}");
            }
            return qs.ToString();
        }

        private static string GetCanonicalRequest(string method, string uri, string queryString, Dictionary<string, string> headers, string signedHeaders, string payload)
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

        static byte[] HmacSHA256(string data, byte[] key)
        {
            var algorithm = "HmacSHA256";
            var kha = KeyedHashAlgorithm.Create(algorithm);
            kha.Key = key;

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        static byte[] Hash(string data)
        {
            var algorithm = "SHA256";
            var kha = HashAlgorithm.Create(algorithm);

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        static string GetHexString(byte[] val)
        {
            var hexString = BitConverter.ToString(val);
            hexString = hexString.Replace("-", "");
            return hexString.ToLower();
        }

        static byte[] getSignatureKey(string key, string dateStamp, string regionName, string serviceName)
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
