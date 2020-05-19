using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.KeyManagementService.Model;
using aws_services.client.Models;
using aws_services.client.Services;
using Microsoft.AspNetCore.Mvc;

namespace aws_services.client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private ICrypto service;

        public CryptoController(ICrypto service)
        {
            this.service = service;
        }

        [HttpPost("encrypt")]
        public Task<string> Encrypt([FromBody] string text)
        {
            return service.Encrypt(text);
        }

        [HttpPost("encryptWithKey")]
        public Task<CryptoData> EncryptWithKey([FromBody] string text)
        {
            return service.EncryptWithKey(text);
        }

        [HttpPost("decrypt")]
        public Task<string> Decrypt([FromBody] string text)
        {
            return service.Decrypt(text);
        }

        [HttpPost("decryptWithKey")]
        public Task<string> DecryptWithKey(CryptoData data)
        {
            return service.DecryptWithKey(data);
        }
    }
}