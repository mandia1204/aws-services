using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace aws_services.Services
{
    public class JobService : IJobService
    {
        private IAmazonSQS sqsClient;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private string queueUrl;
        public JobService()
        {
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.RegionEndpoint = bucketRegion;
            sqsClient = new AmazonSQSClient(sqsConfig);
        }

        private async Task InitializeAsync()
        {
            if(string.IsNullOrEmpty(this.queueUrl))
            {
                this.queueUrl = await GetQueueUrl();
            }
        }

        private async Task<string> GetQueueUrl()
        {
            var request = new GetQueueUrlRequest
            {
                QueueName = "mandia-queue",
                QueueOwnerAWSAccountId = "341189667252"
            };
            var response = await sqsClient.GetQueueUrlAsync(request);
            return response.QueueUrl;
        }

        public async Task<SendMessageResponse> CreateJob(string body)
        { 
            await InitializeAsync();
            var sendMessageRequest = new SendMessageRequest(this.queueUrl, body);

            return await sqsClient.SendMessageAsync(sendMessageRequest);
        }
    }
}
