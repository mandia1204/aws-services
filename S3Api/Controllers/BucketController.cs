using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using S3Api.Services;
using System.Threading.Tasks;

namespace S3Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BucketController : ControllerBase
    {
        private readonly IBucketService _service;

        public BucketController(IBucketService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _service.GetObjects("mandia-storage"));
            }
            catch(AmazonS3Exception s3Exception)
            {
                return StatusCode(500, new { error = s3Exception.Message });
            }
            
        }
    }
}
