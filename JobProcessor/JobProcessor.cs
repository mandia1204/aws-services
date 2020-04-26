using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JobProcessor
{
    public class JobProcessor
    {
        private IAmazonSQS sqsClient;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private string queueUrl;

        public JobProcessor()
        {
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.RegionEndpoint = bucketRegion;
            sqsClient = new AmazonSQSClient(sqsConfig);
        }
        private async Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(this.queueUrl))
            {
                this.queueUrl = await GetQueueUrl();
            }
        }
        public async Task ProcessMessages()
        {
            await InitializeAsync();
            var receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = this.queueUrl;
            receiveMessageRequest.MaxNumberOfMessages = 10;
            receiveMessageRequest.WaitTimeSeconds = 3; //long polling
            var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);

            foreach (var message in response.Messages)
            {
                await ProcessMessage(message);
            }
        }

        private async Task ProcessMessage(Message message)
        {
            Console.WriteLine("message received: {0}", message.Body);
            var deleteMessageRequest = new DeleteMessageRequest();

            deleteMessageRequest.QueueUrl = queueUrl;
            deleteMessageRequest.ReceiptHandle = message.ReceiptHandle;
            await sqsClient.DeleteMessageAsync(deleteMessageRequest);
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
    }
}
