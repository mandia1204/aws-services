using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using aws_services.client.Models;
using aws_services.client.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aws_services.Controllers
{
    [Route("api/[controller]")]
    public class CognitoController : Controller
    {
        private ICognitoService service;

        public CognitoController(ICognitoService service)
        {
            this.service = service;
        }

        //GET api/<controller>/
        [HttpGet]
        public Task<ListUsersResponse> Get()
        {
            return service.ListUsers();
        }

        [HttpPost]
        public Task<AdminUpdateUserAttributesResponse> UpdateUser([FromBody] User user)
        {
            return service.UpdateUser(user);
        }
    }
}
