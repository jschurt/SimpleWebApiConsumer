using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWebApiConsumer.Models
{
    public class ProcessApiViewModel
    {
        public string EndPointUrl { get; set; }
        public string EndPointReturn { get; set; }
        public string ResultStatusCode { get; set; }
        public string Token { get; set; }

    }
}
