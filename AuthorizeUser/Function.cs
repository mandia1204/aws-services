using System;
using System.Linq;
using Amazon.Lambda.Core;
using AuthorizeUser.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AuthorizeUser
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool FunctionHandler(User user, ILambdaContext context)
        {
            var isAuthorized = false;
            if (user.Roles.Contains("admin"))
            {
                isAuthorized = true;
            }
            context.Logger.LogLine($"User: {user.Name}, authorized: {isAuthorized}");
            return isAuthorized;
        }
    }
}
