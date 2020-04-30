using Amazon.DynamoDBv2.Model;
using aws_services.client.Models;
using System.Threading.Tasks;

namespace aws_services.client.Services
{
    public interface IUserService
    {
        Task GetUsers();
        Task<PutItemResponse> SaveUser(User user);
    }
}