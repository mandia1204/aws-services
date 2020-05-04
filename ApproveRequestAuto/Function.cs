using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using ApproveRequestAuto.Models;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ApproveRequestAuto
{
    public class Function
    {
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private IAmazonStepFunctions GetStepFunctionsClient()
        {
            return new AmazonStepFunctionsClient(bucketRegion);
        }
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {

        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach(var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processed message {message.Body}");
            var messageBody = JsonSerializer.Deserialize<TaskMessage>(message.Body);

            context.Logger.LogLine("");
            var stepFunctionsClient = GetStepFunctionsClient();

            var req = new SendTaskSuccessRequest
            {
                Output = "\"Callback task completed successfully(updated again!).\"",
                TaskToken = messageBody.TaskToken
            };
            await stepFunctionsClient.SendTaskSuccessAsync(req);
            
            await Task.CompletedTask;
        }
    }
}
