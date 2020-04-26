using System.Threading.Tasks;
using aws_services.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aws_services.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private IFileStorage service;

        public FileController(IFileStorage service)
        {
            this.service = service;
        }

        // GET api/<controller>/test
        [HttpGet("{fileKey}")]
        public Task<string> Get(string fileKey)
        {
            return service.ReadObject("mandia-storage", fileKey);
        }
    }
}
