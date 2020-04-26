using Amazon.SimpleNotificationService.Model;
using System.Threading.Tasks;

namespace aws_services.Services
{
    public interface IJobServicePubSub
    {
        Task<PublishResponse> CreateJob(string body);
        Task<SubscribeResponse> SubscribeEmail(string email);
        Task<ConfirmSubscriptionResponse> ConfirmSubscription(string token);
        void ProcessNotification(string message);
    }
}