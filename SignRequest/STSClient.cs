using Amazon;
using Amazon.SecurityToken.Model;
using System.Threading.Tasks;

namespace SignRequest
{
    public class STSClient
    {
        private readonly Amazon.SecurityToken.IAmazonSecurityTokenService client;
        private readonly RegionEndpoint region = RegionEndpoint.USEast2;

        public STSClient()
        {
            client = new Amazon.SecurityToken.AmazonSecurityTokenServiceClient(region);
        }

        public Task<AssumeRoleResponse> AssumeRole(string role, string externalId, string roleSessionName)
        {
            var request = new AssumeRoleRequest { 
                DurationSeconds = 3600,
                RoleArn= role,
                ExternalId = externalId,
                RoleSessionName = roleSessionName
            };
            return client.AssumeRoleAsync(request);
        }
    }
}
