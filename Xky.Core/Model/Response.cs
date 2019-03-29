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