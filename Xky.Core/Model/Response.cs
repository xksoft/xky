using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Xky.Core.Model
{
    public class Response
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public JObject Json { get; set; }
    }
}