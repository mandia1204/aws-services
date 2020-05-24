using System;
using System.Linq;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AuthorizeUser.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AuthorizeUserProxy
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var input = JsonSerializer.Serialize(request);
            context.Logger.LogLine($"req: {input}");
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var user = JsonSerializer.Deserialize<User>(request.Body, options);
            var isAuthorized = false;
            if (user.Roles.Contains("admin"))
            {
                isAuthorized = true;
            }
            context.Logger.LogLine($"User: {user.Name}, authorized: {isAuthorized}");
            var response = new APIGatewayProxyResponse
            {
                IsBase64Encoded = false,
                Body = isAuthorized.ToString(),
                StatusCode = 200
            };
            return response;
        }
    }
}
