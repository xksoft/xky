using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xky.Core
{
    public class Client
    {
        public static JObject Post(string api, JObject json)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var httpClient = new HttpClient(handler) {Timeout = new TimeSpan(0, 0, 0, 15)};
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript");
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json.ToString()));
            content.Headers.Add("Content-Type", "application/json");
            var responseMessage = httpClient.PostAsync("https://api.xky.com/" + api, content).Result;
            var jsonResult = JsonConvert.DeserializeObject<JObject>(responseMessage.Content.ReadAsStringAsync().Result);
            if (jsonResult == null || !jsonResult.ContainsKey("encrypt"))
                return new JObject {["errcode"] = 1, ["msg"] = "通讯结果无法解析"};
            return JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()));
        }

        public static JObject Get(string api, JObject json)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var httpClient = new HttpClient(handler) {Timeout = new TimeSpan(0, 0, 0, 15)};
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript");
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json.ToString()));
            content.Headers.Add("Content-Type", "application/json");
            var responseMessage = httpClient.GetAsync("https://api.xky.com/" + api).Result;
            var jsonResult = JsonConvert.DeserializeObject<JObject>(responseMessage.Content.ReadAsStringAsync().Result);
            if (jsonResult == null || !jsonResult.ContainsKey("encrypt"))
                return new JObject {["errcode"] = 1, ["msg"] = "通讯结果无法解析"};
            return JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()));
        }
    }
}