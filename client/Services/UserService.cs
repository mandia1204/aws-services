using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using aws_services.client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aws_services.client.Services
{
    public class UserService : IUserService
    {
        IAmazonDynamoDB client;
        private readonly string TableName = "Users";
        public UserService()
        {
            client = DynamoDbClient.CreateClient();
        }
        public async Task GetUsers()
        {
            var req = new ScanRequest
            {
                TableName = TableName
            };

            var response = await client.ScanAsync(req);

            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                PrintItem(item);
            }
        }

        public Task<PutItemResponse> SaveUser(User user)
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
                TableName = TableName,
                Item = item,
                
            };
            return client.PutItemAsync(req);
        }

        private static void PrintItem(Dictionary<string, AttributeValue> attributeList)
        {
            foreach (KeyValuePair<string, AttributeValue> kvp in attributeList)
            {
                string attributeName = kvp.Key;
                AttributeValue value = kvp.Value;

                Console.WriteLine(
                    attributeName + " " +
                    (value.S == null ? "" : "S=[" + value.S + "]") +
                    (value.N == null ? "" : "N=[" + value.N + "]") +
                    (value.SS == null ? "" : "SS=[" + string.Join(",", value.SS.ToArray()) + "]") +
                    (value.NS == null ? "" : "NS=[" + string.Join(",", value.NS.ToArray()) + "]")
                    );
            }
            Console.WriteLine("************************************************");
        }
    }
}
