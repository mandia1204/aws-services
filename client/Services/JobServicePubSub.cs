using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Threading.Tasks;

namespace aws_services.Services
{
    public class JobServicePubSub : IJobServicePubSub
    {
        private IAmazonSimpleNotificationService snsClient;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private readonly string arn = "arn:aws:sns:us-east-2:341189667252:mandia-topic";
        public JobServicePubSub()
        {
            var config = new AmazonSimpleNotificationServiceConfig();
            config.RegionEndpoint = bucketRegion;
            snsClient = new AmazonSimpleNotificationServiceClient(config);
        }

        public async Task<PublishResponse> CreateJob(string body)
        {
            var req = new PublishRequest();
            req.TopicArn = arn;
            req.Message = body;
            req.Subject = "My notification sent!";

            return await snsClient.PublishAsync(req);
        }

        public async Task<SubscribeResponse> SubscribeEmail(string email)
        {
            var req = new SubscribeRequest();
            req.TopicArn = arn;
            req.Protocol = "email";
            req.Endpoint = email;

            return await snsClient.SubscribeAsync(req);
        }

        public async Task<ConfirmSubscriptionResponse> ConfirmSubscription(string token)
        {
            var req = new ConfirmSubscriptionRequest();
            req.TopicArn = arn;
            req.Token = token;

            return await snsClient.ConfirmSubscriptionAsync(req);
        }

        public void ProcessNotification(string message)
        {
            Console.WriteLine("Received!: {0}", message);
        }

        public async Task<SubscribeResponse> Subscribe(string url)
        {
            var req = new SubscribeRequest();
            req.TopicArn = arn;
            req.Protocol = "http";
            req.Endpoint = url;

            return await snsClient.SubscribeAsync(req);
        }
    }
}
