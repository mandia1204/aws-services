using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Tokens;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaAuthorizer
{
    public class Function
    {
        private const string AppId = "lambda-authorizer.com";
        private const string AllowedUserName = "matt";
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayCustomAuthorizerResponse FunctionHandler(APIGatewayCustomAuthorizerRequest apiRequest, ILambdaContext context)
        {
            bool authorized = false;
            
            if (!string.IsNullOrWhiteSpace(apiRequest.AuthorizationToken))
            {
                authorized = ValidateToken(apiRequest.AuthorizationToken, context);
            }

            var policy = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                {
                    new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                    {
                        Action = new HashSet<string>(new string[]{"execute-api:Invoke"}),
                        Effect =  authorized ? "Allow" : "Deny",
                        Resource = new HashSet<string>(new string[]{apiRequest.MethodArn}) // api gateway endpoint arn
                    }
                }
            };

            var contextOutput = new APIGatewayCustomAuthorizerContextOutput();
            contextOutput["User"] = authorized ? AllowedUserName : "User";
            contextOutput["Path"] = apiRequest.MethodArn;

            var response = new APIGatewayCustomAuthorizerResponse
            {
                Context = contextOutput,
                PolicyDocument = policy
            };
            return response;
        }

        private bool ValidateToken(string token, ILambdaContext context)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var tokenValidationParameters = GetValidationParams();
                var user = handler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                context.Logger.LogLine($"user: {user.Identity.Name}, {user.Identity.AuthenticationType}");
                context.Logger.LogLine($"validatedToken: {JsonSerializer.Serialize(validatedToken)}");

                foreach (var c in user.Claims)
                {
                    context.Logger.LogLine($"claim: {c.Type}, val: {c.Value}");
                }

                var userNameClaim = user.Claims.FirstOrDefault(c => c.Type == "userName");
                if (userNameClaim == null) return false;
                if(userNameClaim.Value != AllowedUserName) return false;
                
                var audClaim = user.Claims.FirstOrDefault(c => c.Type == "aud");
                if (audClaim == null) return false;

                if (!user.Claims.Any(c => c.Type == "aud" && c.Value == AppId)) return false;

                return true;
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message);
                return false;
            }
        }

        private TokenValidationParameters GetValidationParams()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Th1s1sth3endBeatuf@frieND823762873"));
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "security.mattcompany.com",
                ValidAudience = "restaurant.mattcompany.com",
                ClockSkew = TimeSpan.FromMinutes(2),
                IssuerSigningKey = signingKey
            };
        }
    }
}
