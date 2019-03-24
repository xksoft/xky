using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core.Model;
using Quobject.SocketIoClientDotNet.Client;

namespace Xky.Core
{
    public class Client
    {
        internal static async Task<Response> Post(string api, JObject json)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true
                };
                var httpClient = new HttpClient(handler) {Timeout = new TimeSpan(0, 0, 0, 15)};
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript");
                var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json.ToString()));
                content.Headers.Add("Content-Type", "application/json");
                var responseMessage = await httpClient.PostAsync("https://api.xky.com/" + api, content);
                var jsonResult =
                    JsonConvert.DeserializeObject<JObject>(responseMessage.Content.ReadAsStringAsync().Result);
                if (jsonResult == null || !jsonResult.ContainsKey("encrypt"))
                    return new Response
                    {
                        Result = false,
                        Message = "通讯结果无法解析",
                        Json = new JObject {["errcode"] = 1, ["msg"] = "通讯结果无法解析"}
                    };
                var resultJson =
                    JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()));
                return new Response
                {
                    Result = resultJson["errcode"] != null && Convert.ToInt32(resultJson["errcode"]) == 0,
                    Message = resultJson["msg"]?.ToString(),
                    Json = JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()))
                };
            }
            catch (Exception e)
            {
                return new Response
                {
                    Result = false,
                    Message = e.Message,
                    Json = new JObject {["errcode"] = 1, ["msg"] = e.Message}
                };
            }
        }

        internal static async Task<Response> Get(string api, JObject json)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var httpClient = new HttpClient(handler) {Timeout = new TimeSpan(0, 0, 0, 15)};
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript");
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json.ToString()));
            content.Headers.Add("Content-Type", "application/json");
            var responseMessage = await httpClient.GetAsync("https://api.xky.com/" + api);
            var jsonResult = JsonConvert.DeserializeObject<JObject>(responseMessage.Content.ReadAsStringAsync().Result);
            if (jsonResult == null || !jsonResult.ContainsKey("encrypt"))
                return new Response
                {
                    Result = false,
                    Message = "通讯结果无法解析",
                    Json = new JObject {["errcode"] = 1, ["msg"] = "通讯结果无法解析"}
                };
            var resultJson =
                JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()));
            return new Response
            {
                Result = resultJson["errcode"] != null && Convert.ToInt32(resultJson["errcode"]) == 0,
                Message = resultJson["msg"]?.ToString(),
                Json = JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()))
            };
        }

        public static License License;
        public static Socket CoreSocket;

        public static DateTime ConvertTimestamp(double timestamp)
        {
            var converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var newDateTime = converted.AddMilliseconds(timestamp);
            return newDateTime.ToLocalTime();
        }


        public static async Task<Response> AuthLicense(string license)
        {
            try
            {
                var response = await Post("auth_license_key", new JObject {["license_key"] = license});
                if (response.Result)
                {
                    License = new License
                    {
                        Avatra = response.Json["user"]?["t_avatar"]?.ToString(),
                        Email = response.Json["user"]?["t_email"]?.ToString(),
                        Id = (int) response.Json["user"]?["t_id"],
                        LicenseCustom = response.Json["license"]?["t_custom"]?.ToString(),
                        LicenseExpiration = ConvertTimestamp((double) response.Json["license"]?["t_expiration_time"]),
                        LicenseKey = response.Json["user"]?["t_license_key"]?.ToString(),
                        LicenseLevel = (int) response.Json["license"]?["t_level"],
                        LicenseName = response.Json["license"]?["t_name"]?.ToString(),
                        Name = response.Json["user"]?["t_name"]?.ToString(),
                        Phone = response.Json["user"]?["t_phone"]?.ToString(),
                        Session = response.Json["session"]?.ToString()
                    };

                    //释放资源
                    CoreSocket?.Disconnect();
                    CoreSocket?.Off();
                    CoreSocket?.Close();


                    var options = new IO.Options
                    {
                        IgnoreServerCertificateValidation = false,
                        AutoConnect = true,
                        ForceNew = true,
                        Query = new Dictionary<string, string>
                        {
                            {"action", "client"},
                            {"session", License.Session}
                        },
                        Path = "/xky",
                        Transports = ImmutableList.Create("websocket")
                    };
                    CoreSocket = IO.Socket("wss://api.xky.com", options);
                    CoreSocket.On(Socket.EVENT_CONNECT, () => { Console.WriteLine("Connected"); });
                    CoreSocket.On(Socket.EVENT_DISCONNECT, () => { Console.WriteLine("Disconnected"); });
                    CoreSocket.On(Socket.EVENT_ERROR, () => { Console.WriteLine("ERROR"); });
                    CoreSocket.On("event", json => { Console.WriteLine(json); });
                }
                else
                {
                    License = null;
                }

                return response;
            }
            catch (Exception e)
            {
                return new Response
                {
                    Result = false,
                    Message = e.Message,
                    Json = new JObject {["errcode"] = 1, ["msg"] = e.Message}
                };
            }
        }
    }
}