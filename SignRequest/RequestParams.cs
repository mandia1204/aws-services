using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SignRequest
{
    public class RequestParams
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
        public string securityToken { get; set; } // used for assumed roles
        public string encodedSecurityToken
        {
            get
            {
                return WebUtility.UrlEncode(securityToken);
            }
        }
        public bool useAuthorizationHeader { get; set; }
        public string encodedCredentials
        {
            get
            {
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
        public string credentialScope
        {
            get
            {
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
}
