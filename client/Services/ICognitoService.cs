using Amazon.CognitoIdentityProvider.Model;
using aws_services.client.Models;
using System.Threading.Tasks;

namespace aws_services.client.Services
{
    public interface ICognitoService
    {
        Task<AdminUpdateUserAttributesResponse> UpdateUser(User user);
        Task<ListUsersResponse> ListUsers();
    }
}