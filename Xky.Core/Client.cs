using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using Xky.Core.Common;
using Xky.Core.Model;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;


namespace Xky.Core
{
    public static class Client
    {
        internal static Socket CoreSocket;


        internal static Response Post(string api, JObject json)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true
                };
                var httpClient = new HttpClient(handler) {Timeout = TimeSpan.FromSeconds(15)};
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript");
                var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json.ToString()));
                content.Headers.Add("Content-Type", "application/json");
                var responseMessage = httpClient.PostAsync("https://api.xky.com/" + api, content).Result;
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
            var httpClient = new HttpClient(handler) {Timeout = TimeSpan.FromSeconds(15)};
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

        public static void SearchLocalNode()
        {
            StartAction(() =>
            {
                var udp = new UdpClient(18866);
                var ip = new IPEndPoint(IPAddress.Any, 18866);
                while (true)
                {
                    var bytes = udp.Receive(ref ip);
                    var json = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(bytes));
                    var serial = json["serial"]?.ToString();
                    if (serial != null)
                    {
                        if (!LocalNodes.ContainsKey(serial))
                        {
                            Console.WriteLine("找到局域网节点" + serial);
                            LocalNodes.Add(serial, new Node());
                        }

                        LocalNodes[serial].Serial = json["serial"]?.ToString();
                        LocalNodes[serial].Name = json["name"].ToString();
                        LocalNodes[serial].Ip = ip.Address.ToString();
                        LocalNodes[serial].LoadTick = DateTime.Now.Ticks;

                        lock ("nodes")
                        {
                            var node = Nodes.ToList().Find(p => p.Serial == serial);
                            if (node != null)
                            {
                                ConnectToNode(node, "http://" + LocalNodes[serial].Ip + ":8080",
                                    node.ConnectionHash);
                            }
                        }
                    }
                }
            });
        }


        /// <summary>
        ///     认证授权KEY
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        public static Response AuthLicense(string license)
        {
            try
            {
                var response = Post("auth_license_key", new JObject {["license_key"] = license});
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
                    CoreSocket.On(Socket.EVENT_CONNECT, () =>
                    {
                        Console.WriteLine("Connected");
                        CoreConnected = true;
                    });
                    CoreSocket.On(Socket.EVENT_DISCONNECT, () =>
                    {
                        Console.WriteLine("Disconnected");
                        CoreConnected = false;
                    });
                    CoreSocket.On(Socket.EVENT_ERROR, () => { Console.WriteLine("ERROR"); });
                    CoreSocket.On("event", json => { CoreEvent((JObject) json); });
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

        /// <summary>
        ///     重新加载设备列表
        /// </summary>
        /// <returns></returns>
        public static Response LoadDevices()
        {
            try
            {
                if (License == null)
                    return new Response
                    {
                        Result = false,
                        Message = "未授权",
                        Json = new JObject {["errcode"] = 1, ["msg"] = "未授权"}
                    };

                var loadtick = DateTime.Now.Ticks;
                var response = Post("get_device_list", new JObject {["session"] = License.Session});

                if (response.Result)
                {
                    foreach (var json in (JArray) response.Json["list"]) PushDevice(json, loadtick);

                    //删除所有本时序中不存在的设备 用UI线程委托删除，防止报错
                    MainWindow.Dispatcher.Invoke(() =>
                    {
                        foreach (var t in Devices.ToList())
                            if (t.LoadTick != loadtick)
                                Devices.Remove(t);
                    });
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

        /// <summary>
        ///     获取设备连接信息
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static Device GetDevice(string sn)
        {
            var response = Post("get_device",
                new JObject {["sn"] = sn, ["session"] = License.Session});
            if (!response.Result)
            {
                Console.WriteLine(response.Message);
            }

            return !response.Result ? null : PushDevice(response.Json, DateTime.Now.Ticks);
        }

        /// <summary>
        ///     启动一个任务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Thread StartAction(Action action)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    lock ("thread_count")
                    {
                        Threads++;
                    }

                    action.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    lock ("thread_count")
                    {
                        Threads--;
                    }
                }
            }) {IsBackground = true};
            thread.Start();
            return thread;
        }


        /// <summary>
        ///     重新加载模块列表
        /// </summary>
        /// <returns></returns>
        public static Response LoadModules_Panel()
        {
            try
            {
                if (License == null)
                    return new Response
                    {
                        Result = false,
                        Message = "未授权",
                        Json = new JObject {["errcode"] = 1, ["msg"] = "未授权"}
                    };
                var response = Post("get_module_panel", new JObject {["session"] = License.Session});

                if (response.Result)
                {
                    foreach (var json in (JArray) response.Json["result"]) PushModule(json);
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

        #region 公开属性

        public static License License;
        public static bool CoreConnected;
        public static Window MainWindow;

        public static int Threads;

        public static readonly ObservableCollection<Node> Nodes = new ObservableCollection<Node>();
        public static readonly Dictionary<string, Node> LocalNodes = new Dictionary<string, Node>();
        public static readonly ObservableCollection<Tag> Tags = new ObservableCollection<Tag>();
        public static readonly ObservableCollection<Device> Devices = new ObservableCollection<Device>();
        public static readonly ObservableCollection<Module> Modules_Panel = new ObservableCollection<Module>();

        public static readonly ObservableCollection<string> Modules_Panel_Tags =
            new ObservableCollection<string> {"所有模块"};

        public static AverageNumber BitAverageNumber = new AverageNumber(3);

        #endregion


        #region  内部方法

        private static void ConnectToNode(Node node, string url, string hash)
        {
            lock ("node_connect")
            {
                if (node.ConnectStatus > 0 && node.NodeUrl == url)
                {
                    return;
                }

                StartAction(() =>
                {
                    node.NodeUrl = url;


                    node.NodeSocket?.Close();


                    var options = new IO.Options
                    {
                        IgnoreServerCertificateValidation = true,
                        AutoConnect = true,
                        ForceNew = true,
                        Query = new Dictionary<string, string>
                        {
                            {"hash", hash},
                            {"action", "client"}
                        },
                        Path = "/xky",
                        Transports = ImmutableList.Create("websocket")
                    };

                    node.NodeSocket = IO.Socket(url, options);
                    node.NodeSocket.On(Socket.EVENT_CONNECT, () =>
                    {
                        node.NodeSocket.Emit("hello", (oo) => { Console.WriteLine(oo); });

                        node.ConnectStatus = url.Contains("xxapi.org") ? 1 : 2;
                        Console.WriteLine("node Connected " + url);
                    });
                    node.NodeSocket.On(Socket.EVENT_DISCONNECT, () =>
                    {
                        node.ConnectStatus = 0;
                        Console.WriteLine("node Disconnected");
                    });
                    node.NodeSocket.On(Socket.EVENT_ERROR, () => { Console.WriteLine("node ERROR"); });
                    node.NodeSocket.On("event", json => { Console.WriteLine(json); });
                    node.NodeSocket.On("img",
                        new MyListenerImpl((sn, data) =>
                        {
                            //   Console.WriteLine("收到截图" + sn + " 长度:" + ((byte[]) data).Length);
                            var imgdata = (byte[]) data;
                            //加入速率计数器
                            BitAverageNumber.Push(imgdata.Length);
                            var device = Devices.ToList().Find(p => p.Sn == (string) sn);
                            if (device != null)
                            {
                                device.ScreenShot = ByteToBitmapSource((byte[]) data);
                            }
                        }));
                });
            }
        }


        /// <summary>
        ///     unix时间戳转换成datetime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private static DateTime ConvertTimestamp(double timestamp)
        {
            var converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var newDateTime = converted.AddMilliseconds(timestamp);
            return newDateTime.ToLocalTime();
        }

        /// <summary>
        ///     添加或更新Device
        /// </summary>
        /// <param name="json"></param>
        /// <param name="loadtick"></param>
        private static Device PushDevice(JToken json, long loadtick)
        {
            lock ("devices")
            {
                var device = Devices.ToList().Find(p => p.Id == (int) json["t_id"]);
                //如果已经存在就更新
                if (device != null)
                {
                    device.ConnectionHash = json["t_connection_hash"]?.ToString();
                    device.Description = json["t_desc"]?.ToString();
                    device.Forward = json["t_forward"]?.ToString();
                    device.NodeUrl = json["t_nodeurl"]?.ToString();
                    device.NodeSerial = json["t_node"]?.ToString();
                    device.GpsLat = json["t_gps_lat"]?.ToString();
                    device.GpsLng = json["t_gps_lng"]?.ToString();
                    device.Id = (int) json["t_id"];
                    device.Model = json["t_model"]?.ToString();
                    device.Name = json["t_name"]?.ToString();
                    device.Node = json["t_node"]?.ToString();
                    device.Product = json["t_product"]?.ToString();
                    device.Sn = json["t_sn"]?.ToString();
                    device.Cpus = (int) json["t_cpus"];
                    device.Memory = (int) json["t_memory"];
                    device.LoadTick = loadtick;
                }
                else
                {
                    device = new Device
                    {
                        ConnectionHash = json["t_connection_hash"]?.ToString(),
                        Description = json["t_desc"]?.ToString(),
                        Forward = json["t_forward"]?.ToString(),
                        NodeUrl = json["t_nodeurl"]?.ToString(),
                        NodeSerial = json["t_node"]?.ToString(),
                        GpsLat = json["t_gps_lat"]?.ToString(),
                        GpsLng = json["t_gps_lng"]?.ToString(),
                        Id = (int) json["t_id"],
                        Model = json["t_model"]?.ToString(),
                        Name = json["t_name"]?.ToString(),
                        Node = json["t_node"]?.ToString(),
                        Product = json["t_product"]?.ToString(),
                        Sn = json["t_sn"]?.ToString(),
                        Cpus = (int) json["t_cpus"],
                        Memory = (int) json["t_memory"],
                        LoadTick = loadtick
                    };

                    StartAction(() =>
                    {
                        try
                        {
                            using (var client = new WebClient())
                            {
                                var data = client.DownloadData("http://static.xky.com/screenshot/" + device.Sn +
                                                               ".jpg?x-oss-process=image/resize,h_100,w_52");
                                MainWindow.Dispatcher.Invoke(() => { device.ScreenShot = ByteToBitmapSource(data); });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });

                    //添加节点服务器
                    StartAction(() => { PushNode(device.NodeSerial); });


                    //用UI线程委托添加，防止报错
                    MainWindow.Dispatcher.Invoke(() => { Devices.Add(device); });
                }

                return device;
            }
        }

        /// <summary>
        ///     添加或更新模块面板
        /// </summary>
        /// <param name="json"></param>
        private static Module PushModule(JToken json)
        {
            lock ("devices")
            {
                var module = Modules_Panel.ToList().Find(p => p.Id == (int) json["t_id"]);
                //如果已经存在就更新
                if (module != null)
                {
                    module.Id = (int) json["t_id"];
                    module.Name = json["t_name"]?.ToString();

                    module.Price = (int) json["t_price"];
                    module.Status = (int) json["t_status"];
                    module.Uid = (int) json["t_uid"];
                    module.Type = (int) json["t_type"];
                    module.Tags = json["t_tags"].Values<string>().ToList();
                }
                else
                {
                    module = new Module
                    {
                        Id = (int) json["t_id"],
                        Name = json["t_name"]?.ToString(),

                        Price = (int) json["t_price"],
                        Status = (int) json["t_status"],
                        Uid = (int) json["t_uid"],
                        Type = (int) json["t_type"],
                        Tags = json["t_tags"].Values<string>().ToList()
                    };
                    foreach (string tag in module.Tags)
                    {
                        if (!Modules_Panel_Tags.Contains(tag))
                        {
                            Modules_Panel_Tags.Add(tag);
                        }
                    }

                    StartAction(() =>
                    {
//                        //得加个锁，不然一下子并发上百个请求，会被服务器堵住
//                        lock ("down_module_image")
//                        {
//                            try
//                            {
//                                using (var client = new WebClient())
//                                {
//                                    var data = client.DownloadData(json["t_logo"] + "@96h");
//                                    MainWindow.Dispatcher.Invoke(() => { module.Logo = ByteToBitmapSource(data); });
//                                }
//                            }
//                            catch (Exception e)
//                            {
//                                Console.WriteLine(e);
//                            }
//                        }
                    });
                    MainWindow.Dispatcher.Invoke(() =>
                    {
                        //设置初始屏幕
                        module.Logo =
                            new BitmapImage(new Uri(json["t_logo"] + "@96h"));
                        Modules_Panel.Add(module);
                    });
                }

                return module;
            }
        }

        /// <summary>
        ///     移除Device
        /// </summary>
        /// <param name="json"></param>
        private static void RemoveDevice(JToken json)
        {
            lock ("devices")
            {
                var device = Devices.ToList().Find(p => p.Id == (int) json["t_id"]);
                if (device != null) MainWindow.Dispatcher.Invoke(() => { Devices.Remove(device); });
            }
        }


        private static Node PushNode(string serial)
        {
            lock ("nodes")
            {
                var node = Nodes.ToList().Find(p => p.Serial == serial);
                if (node != null)
                    return node;


                var response = Post("get_node", new JObject {["session"] = License.Session, ["serial"] = serial});

                if (!response.Result) return null;

                var json = response.Json["node"];

                node = new Node
                {
                    Serial = json["t_serial"]?.ToString(),
                    Name = json["t_name"]?.ToString(),
                    ConnectionHash = json["t_connection_hash"]?.ToString(),
                    Forward = json["t_forward"]?.ToString(),
                    DeviceCount = int.Parse(json["t_online_devices"].ToString()),
                    Ip = json["t_ip"]?.ToString()
                };

                ConnectToNode(node, json["t_nodeurl"]?.ToString(), node.ConnectionHash);


                //用UI线程委托添加，防止报错
                MainWindow.Dispatcher.Invoke(() => { Nodes.Add(node); });
                return node;
            }
        }

        /// <summary>
        ///     核心服务器事件
        /// </summary>
        /// <param name="json"></param>
        private static void CoreEvent(JObject json)
        {
            var type = json["type"]?.ToString();
            switch (type)
            {
                case "device_state":
                {
                    if (json["message"]?.ToString() == "online")
                        PushDevice(json["device"], DateTime.Now.Ticks);
                    else
                        RemoveDevice(json["device"]);

                    break;
                }
            }
        }


        /// <summary>
        ///     byte转图片源
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static BitmapSource ByteToBitmapSource(byte[] buffer)
        {
            var image = new BitmapImage();
            using (var stream = new MemoryStream(buffer))
            {
                stream.Seek(0, SeekOrigin.Begin);
                image.BeginInit();
                image.StreamSource = stream;
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;

                //image.DecodePixelWidth = 100;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }

        #endregion
    }
}