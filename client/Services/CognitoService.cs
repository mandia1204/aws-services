using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using aws_services.client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aws_services.client.Services
{
    public class CognitoService : ICognitoService
    {
        private readonly IAmazonCognitoIdentityProvider client;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private readonly string userPoolId = "us-east-2_05E5JDNR9";
        public CognitoService()
        {
            client = new AmazonCognitoIdentityProviderClient(bucketRegion);
        }

        public Task<AdminUpdateUserAttributesResponse> UpdateUser(User user)
        {
            var attrs = new List<AttributeType>();
            attrs.Add(new AttributeType { 
                Name = "custom:department",
                Value = user.Department
            });
            var req = new AdminUpdateUserAttributesRequest
            {
                Username = user.Name,
                UserPoolId = userPoolId,
                UserAttributes = attrs
            };
            return client.AdminUpdateUserAttributesAsync(req);
        }

        public Task<ListUsersResponse> ListUsers()
        {
            var req = new ListUsersRequest
            {
                UserPoolId = userPoolId
            };
            return client.ListUsersAsync(req);
        }
    }
}
