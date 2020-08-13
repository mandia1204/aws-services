using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateUser
{
    public class Function
    {

        private IAmazonSimpleNotificationService snsClient;
        private IAmazonDynamoDB dynamoClient;
        private readonly RegionEndpoint region = RegionEndpoint.USEast2;
        private readonly string arn = "arn:aws:sns:us-east-2:341189667252:create-user";
        //private static AmazonLambdaClient lambdaClient;

        public Function()
        {
            // xray = new AmazonXRayClient(region);
            //var opts = new XRayOptions();
            //IAWSXRayRecorder recorder = new AWSXRayRecorder();
            initialize();
            snsClient = new AmazonSimpleNotificationServiceClient(region);
            dynamoClient = new AmazonDynamoDBClient(region);
        }

        static void initialize()
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            //lambdaClient = new AmazonLambdaClient();
            //await callLambda();
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(User user, ILambdaContext context)
        {
            //save to db
            context.Logger.LogLine($"user { user.Id} {user.Name} {user.LastName}  {user.Department}");

            var result = await SaveUser(user);
            context.Logger.LogLine("result from dynamo: " + result.HttpStatusCode.ToString());

            //send notification
            var serializationOptions = new JsonSerializerOptions();
            serializationOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            var userJson = JsonSerializer.Serialize(user, serializationOptions);
            context.Logger.LogLine(userJson);
            user.Default = "null";
            var req = new PublishRequest();
            req.TopicArn = arn;
            req.MessageStructure = "json";
            req.Message = userJson;
            req.Subject = "My notification sent!";
            var response = await snsClient.PublishAsync(req);

            context.Logger.LogLine(response.HttpStatusCode.ToString());
            context.Logger.LogLine(response.MessageId);
        }

        private Task<PutItemResponse> SaveUser(User user)
        {
            var ttl = DateTimeOffset.Now.AddMinutes(1).ToUnixTimeSeconds();
            var item = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue() { N = user.Id.ToString() } },
                { "name", new AttributeValue() { S = user.Name } },
                { "lastName", new AttributeValue() { S = user.LastName } },
                { "ttl", new AttributeValue() { N = ttl.ToString() } }
            };
            var req = new PutItemRequest
            {
                TableName = "Users",
                Item = item,

            };
            return dynamoClient.PutItemAsync(req);
        }

        //public static async Task<GetAccountSettingsResponse> callLambda()
        //{
        //    var request = new GetAccountSettingsRequest();
        //    var response = await lambdaClient.GetAccountSettingsAsync(request);
        //    return response;
        //}
    }
}
