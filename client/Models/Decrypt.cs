using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aws_services.client.Models
{
    public class Decrypt
    {
        public string Encrypted { get; set; }
        public string Key { get; set; }
    }
}
