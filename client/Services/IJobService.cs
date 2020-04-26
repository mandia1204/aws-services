using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace aws_services.Services
{
    public interface IJobService
    {
        Task<SendMessageResponse> CreateJob(string body);
    }
}