using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core.Model;

namespace Xky.Core.Common
{
    public class HttpHelper
    {
        public static string Get(string url)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true
                };
                var httpClient = new HttpClient(handler) {Timeout = TimeSpan.FromSeconds(15)};
                return httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}