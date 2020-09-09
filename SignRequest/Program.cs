using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SignRequest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var responseMessage = await AssumeRole("arn:aws:iam::341189667252:role/IAMRole", "testing");
            var creds = responseMessage.Credentials;
            var listUsersResponse = await SendSignedRequest(creds.SecretAccessKey, creds.AccessKeyId, creds.SessionToken);

            Console.WriteLine(listUsersResponse);
            Console.ReadLine();
        }

        private static async Task<string> SendSignedRequest(string secret, string accessKeyId, string securityToken)
        {
            var signRequestService = new SignRequestService();
            var currDate = DateTime.Now.ToUniversalTime();
            var amzdate = $"{currDate:yyyyMMdd}T{currDate:HHmmss}Z";
            var requestParams = new RequestParams
            {
                secret = secret,
                accessKeyId = accessKeyId,
                region = "us-east-1",
                service = "iam",
                action = "ListUsers",
                algorithm = "AWS4-HMAC-SHA256",
                amzdate = amzdate,
                date = currDate.ToString("yyyyMMdd"),
                version = "2010-05-08",
                payload = "",
                securityToken = securityToken,
                useAuthorizationHeader = false,
                headers = new Dictionary<string, string>
                {
                    //{ "content-type", "application/x-www-form-urlencoded; charset=utf-8" },
                    { "host", "iam.amazonaws.com" },
                    { "x-amz-date", amzdate }
                }
            };

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://iam.amazonaws.com");
            var request = signRequestService.GetSignedRequest(requestParams);
            var response = await client.SendAsync(request);
            var responseMessage = await response.Content.ReadAsStringAsync();
            return responseMessage;
        }

        private static Task<AssumeRoleResponse> AssumeRole(string roleArn, string sessionName)
        {
            var client = new STSClient();
            var externalId = "mysecret";
            return client.AssumeRole(roleArn, externalId, sessionName);
        }
    }
}
