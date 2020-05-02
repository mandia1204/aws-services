using Amazon;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using ApproveRequestActivityWorker.Models;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ApproveRequestActivityWorker
{
    public class Function
    {
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private IAmazonStepFunctions GetStepFunctionsClient()
        {
            return new AmazonStepFunctionsClient(bucketRegion);
        }
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(ILambdaContext context)
        {
            var client = GetStepFunctionsClient();
            var snsClient = new AmazonSimpleNotificationServiceClient(bucketRegion); ;

            var req = new GetActivityTaskRequest()
            {
                ActivityArn = "arn:aws:states:us-east-2:341189667252:activity:ApproveRequest"
            };
            var response = await client.GetActivityTaskAsync(req);

            if (response == null || response.Input == null)
            {
                return "No activities received after 60 seconds.";
            }

            var approveData = JsonSerializer.Deserialize<ApproveData>(response.Input);
            var token = WebUtility.UrlEncode(response.TaskToken);
            var emailBody = "can you approve? ," + "https://8tj14zkhq2.execute-api.us-east-2.amazonaws.com/respond/succeed?taskToken=" + token +" or reject: " + "https://8tj14zkhq2.execute-api.us-east-2.amazonaws.com/respond/fail?taskToken=" + token;
            var pubReq = new PublishRequest();
            pubReq.TopicArn = "arn:aws:sns:us-east-2:341189667252:approval-request";
            pubReq.Message = emailBody;
            pubReq.Subject = "Need your confirmation!";

            try
            {
                await snsClient.PublishAsync(pubReq);
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return "error in sending notification:" + ex.Message;
            }

            return "worker completed!";
        }
    }
}
