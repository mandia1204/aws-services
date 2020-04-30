using Amazon;
using Amazon.DynamoDBv2;

namespace aws_services.client
{
    public static class DynamoDbClient
    {
        public static IAmazonDynamoDB CreateClient()
        {
            var config = new AmazonDynamoDBConfig {
                RegionEndpoint = RegionEndpoint.USEast2,
            };
            
            var client = new AmazonDynamoDBClient(config);
            return client;
        }
    }
}
