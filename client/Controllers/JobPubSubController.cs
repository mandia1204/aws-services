using System.Text.Json;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using aws_services.client.Models;
using aws_services.Services;
using Microsoft.AspNetCore.Mvc;

namespace aws_services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPubSubController : ControllerBase
    {
        private IJobServicePubSub service;

        public JobPubSubController(IJobServicePubSub service)
        {
            this.service = service;
        }

        [HttpPost]
        public Task<PublishResponse> CreateJob([FromBody] string body)
        {
            return service.CreateJob(body);
        }

        [HttpPost("email")]
        public Task<SubscribeResponse> SubscribeEmail([FromBody] string email)
        {
            return service.SubscribeEmail(email);
        }

        [HttpPost("notification")]
        [Consumes("text/plain")]
        public async Task<string> Notification([FromBody] string body)
        {
            var messageType = HttpContext.Request.Headers["x-amz-sns-message-type"];
            if (messageType == "SubscriptionConfirmation")
            {
                var confirmation = JsonSerializer.Deserialize<SubscriptionConfirmation>(body);
                var response = await service.ConfirmSubscription(confirmation.Token);
                return response.SubscriptionArn;
            }
            else if (messageType == "Notification")
            {
                var notification = JsonSerializer.Deserialize<Notification>(body);
                service.ProcessNotification(notification.Message);
                return "Notification processed!";
            }

            return "invalid type";
        }
    }
}