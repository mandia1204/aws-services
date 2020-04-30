using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using aws_services.client.Models;
using aws_services.client.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aws_services.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        // GET api/<controller>/
        [HttpGet]
        public async Task<string> Get()
        {
            await service.GetUsers();
            return "OK";
        }

        [HttpPost]
        public Task<PutItemResponse> SaveUser([FromBody] User user)
        {
            return service.SaveUser(user);
        }
    }
}
