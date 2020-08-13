using Amazon.Lambda.Core;
using Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProcessS3BucketChangeCloudWatch
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool FunctionHandler(Amazon.Lambda.CloudWatchEvents.CloudWatchEvent<CloudTrailS3Event> input, ILambdaContext context)
        {
            context.Logger.LogLine($"id {input.Id}");
            context.Logger.LogLine($"region {input.Region}");
            context.Logger.LogLine($"source {input.Source}");
            context.Logger.LogLine($"eventName {input.Detail.EventName}");
            return true;
        }
    }
}
