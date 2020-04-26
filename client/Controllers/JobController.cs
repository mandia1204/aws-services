using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using aws_services.Services;
using Microsoft.AspNetCore.Mvc;

namespace aws_services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private IJobService service;

        public JobController(IJobService service)
        {
            this.service = service;
        }

        [HttpPost]
        public Task<SendMessageResponse> CreateJob([FromBody] string body)
        {
            return service.CreateJob(body);
        }
    }
}