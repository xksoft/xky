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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core.Common;
using Xky.Core.Model;
using Xky.Socket.Client;

namespace Xky.Core
{
    /// <summary>
    /// 客户端
    /// </summary>
    public static class Client
    {
        private static Socket.Client.Socket _coreSocket;

        private static string _lastSearchKeyword;
        private static string _lastSearchTag;
        public static string ModulePath = "";
        public static bool BatchControl = false;
        public static Tag BatchControlTag = null;
        #region 公开属性


        /// <summary>
        /// 授权信息
        /// </summary>
        public static License License;

        /// <summary>
        /// 是否连接核心
        /// </summary>
        public static bool CoreConnected;

        /// <summary>
        /// 主ui
        /// </summary>
        public static Window MainWindow;

        /// <summary>
        /// 并发线程列表
        /// </summary>
        public static int ThreadCount = 0;

        /// <summary>
        /// 节点信息
        /// </summary>
        public static readonly ObservableCollection<Node> Nodes = new ObservableCollection<Node>();

        /// <summary>
        /// 标签信息
        /// </summary>
        public static ObservableCollection<Tag> Tags = new ObservableCollection<Tag>()
            {new Tag() {Count = 0, Devices = new List<Device>(), Name = "所有设备"}};

        /// <summary>
        /// 节点标签信息
        /// </summary>
        public static ObservableCollection<Tag> NodeTags = new ObservableCollection<Tag>();

        /// <summary>
        /// 所有设备
        /// </summary>
        public static readonly ObservableCollection<Device> Devices = new ObservableCollection<Device>();

        /// <summary>
        /// 面板上显示的设备
        /// </summary>
        public static ObservableCollection<Device> PanelDevices = new ObservableCollection<Device>();

        /// <summary>
        /// 模块列表
        /// </summary>
        public static ObservableCollection<Module> Modules = new ObservableCollection<Module>();

        /// <summary>
        /// 速率计数器
        /// </summary>
        public static AverageNumber BitAverageNumber = new AverageNumber(3);

        /// <summary>
        /// 局域网节点发现监听
        /// </summary>
        public static UdpClient UdpClientSearchNode;

        #endregion

        #region 全局方法

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
                        Session = response.Json["session"]?.ToString(),
                        NickName= response.Json["user"]?["t_nickname"]?.ToString()
                    };

                    //释放资源
                    _coreSocket?.Disconnect();
                    _coreSocket?.Off();
                    _coreSocket?.Close();


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
                    _coreSocket = IO.Socket("wss://api.xky.com", options);
                    _coreSocket.On(Socket.Client.Socket.EventConnect, () =>
                    {
                        Console.WriteLine("Connected");
                        CoreConnected = true;
                    });
                    _coreSocket.On(Socket.Client.Socket.EventDisconnect, () =>
                    {
                        Console.WriteLine("Disconnected");
                        CoreConnected = false;
                    });
                    _coreSocket.On(Socket.Client.Socket.EventError, () => { Console.WriteLine("ERROR"); });
                    _coreSocket.On("event", json => { CoreEvent((JObject) json); });
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
        ///     启动一个任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Thread StartAction(Action action, ApartmentState state = ApartmentState.MTA)
        {
          
            var thread = new Thread(() =>
                {
                    try
                    {
                        lock ("thread_count")
                        {
                            ThreadCount++;
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
                            ThreadCount--;
                        }
                        
                    }
                })
            { IsBackground = true};
            thread.SetApartmentState(state);
            thread.Start();
          
               
            return thread;
        }

        /// <summary>
        /// 加载模块
        /// </summary>
        public static void LoadModules()
        {
            var currentfilename = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            ModulePath = currentfilename?.Remove(currentfilename.LastIndexOf("\\", StringComparison.Ordinal)) +
                             "\\Modules";
            var groupnamepaths = Directory.GetDirectories(ModulePath);
            foreach (var groupnamepath in groupnamepaths)
            {
                var groupname = new DirectoryInfo(groupnamepath).Name;
                var modulefilelist = FileHelper.GetFileList(groupnamepath, "*.dll", true);
                foreach (var modulefile in modulefilelist)
                {
                    try
                    {
                        var xmodulelist = XModuleHelper.LoadXModules(modulefile);
                        foreach (var xmodule in xmodulelist)
                        {
                            var modulecontent = (XModule) xmodule.Clone();
                            var module = new Module
                            {
                                Md5 = StrHelper.Md5(groupnamepath + modulecontent.GetType().FullName, false),
                                Name = modulecontent.Name(),
                                GroupName = groupname,
                                Description = modulecontent.Description(),
                                XModule = modulecontent,
                                Icon = modulecontent.Icon()
                            };
                            Client.Modules.Add(module);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("模块加载失败：" + e);
                    }
                }
            }
        }

        #endregion

        #region  节点相关

        /// <summary>
        /// 查找本地节点
        /// </summary>
        public static void SearchLocalNode()
        {
            StartAction(() =>
            {
                UdpClientSearchNode = new UdpClient(18866);
                var ip = new IPEndPoint(IPAddress.Any, 18866);
                while (true)
                {
                    var bytes = UdpClientSearchNode.Receive(ref ip);
                    var json = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(bytes));
                    var serial = json["serial"]?.ToString();
                    if (serial != null)
                    {
                        var node = new Node
                        {
                            Serial = json["serial"]?.ToString(),
                            Name = json["name"]?.ToString(),
                            Ip = ip.Address.ToString(),
                            LoadTick = DateTime.Now.Ticks
                        };


                        PushNode(node, true);
                    }
                }
            });
        }

        /// <summary>
        /// 加载节点列表
        /// </summary>
        /// <returns></returns>
        public static Response LoadNodes()
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

                var response = CallApi("get_node_list", new JObject());

                if (response.Result)
                {
                    Console.WriteLine(response);
                    foreach (var json in (JArray) response.Json["nodes"])
                    {
                        PushNode(GetNode(json["t_serial"].ToString()), false);
                    }
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
        /// 删除节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Response DeleteNode(int id)
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


                var response = CallApi("del_node", new JObject {["id"] = id});
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
        /// 修改节点名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Response SetNode(int id, string name)
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


                var response = CallApi("set_node", new JObject {["id"] = id, ["name"] = name});
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
        /// 添加节点（将节点绑定到当前授权上）
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Response AddNode(string serial, string name)
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


                var response = CallApi("add_node", new JObject {["serial"] = serial, ["name"] = name});
                if (response.Result && response.Json["errcode"].ToString() == "0")
                {
                    var node = GetNode(serial);
                    PushNode(node, false);
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
        /// 从当前节点列表中移除
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveNode(int id)
        {
            lock ("nodes")
            {
                var node = Nodes.ToList().Find(p => p.Id == id);
                if (node != null)
                {
                    if (node.NodeSocket != null)
                    {
                        try
                        {
                            node.NodeSocket.Close();
                        }
                        catch
                        {
                        }
                    }

                    MainWindow.Dispatcher.Invoke(() => { Nodes.Remove(node); });
                }
            }
        }

        /// <summary>
        /// 获取节点详细信息
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        private static Node GetNode(string serial)
        {
            lock ("nodes")
            {
                var response = CallApi("get_node", new JObject {["serial"] = serial});

                if (!response.Result) return null;

                var json = response.Json["node"];

                var node = new Node
                {
                    Serial = json["t_serial"]?.ToString(),
                    Name = json["t_name"]?.ToString(),
                    ConnectionHash = json["t_connection_hash"]?.ToString(),
                    Forward = json["t_forward"]?.ToString(),
                    DeviceCount = int.Parse(json["t_online_devices"].ToString()),
                    Ip = json["t_ip"]?.ToString(),
                    Id = json["t_id"] == null ? 0 : Convert.ToInt32(json["t_id"]),
                    NodeUrl = json["t_nodeurl"]?.ToString()
                };

                //ConnectToNode(node, json["t_nodeurl"]?.ToString(), node.ConnectionHash);

                return node;
            }
        }

        private static void PushNode(Node n, bool local)
        {
            lock ("nodes")
            {
                var node = Nodes.ToList().Find(p => p.Serial == n.Serial);
                if (node != null)
                {
                    if (node.ConnectionHash != null && local)
                    {
                        //授权节点在当前局域网中，自动切换到局域网模式
                        node.Ip = n.Ip;
                        ConnectToNode(node,
                            "http://" + (string.IsNullOrEmpty(node.Ip) ? "127.0.0.1" : node.Ip + ":8080"),
                            node.ConnectionHash);

                        return;
                    }
                    else if (node.ConnectionHash != null && n.ConnectionHash == null)
                    {
                        return;
                    }
                    else
                    {
                        node.ConnectionHash = n.ConnectionHash;
                        node.ConnectStatus = n.ConnectStatus;
                        node.DeviceCount = n.DeviceCount;
                        node.Forward = n.Forward;
                        node.Id = n.Id;
                        node.Ip = n.Ip;
                        node.LoadTick = n.LoadTick;
                        node.Name = n.Name;
                        node.NodeUrl = n.NodeUrl;
                        node.Serial = n.Serial;
                    }
                }
                else
                {
                    if (n.ConnectionHash != null)
                    {
                        if (!local)
                        {
                            //自动通过外网连接当前节点
                            if (!string.IsNullOrEmpty(n.NodeUrl))
                            {
                                ConnectToNode(n, n.NodeUrl, n.ConnectionHash);
                            }
                            else
                            {
                                //节点未设置p2p端口，尚未在局域网发现该节点
                            }
                        }
                    }


                    MainWindow.Dispatcher.Invoke(() => { Nodes.Add(n); });
                }
            }
        }

        #endregion

        #region 设备相关

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

                var response = CallApi("get_device_list", new JObject());

                if (response.Result)
                {
                    foreach (var json in (JArray) response.Json["list"])
                    {
                        PushDevice(json);
                    }
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
        /// 搜索设备
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="tag"></param>
        /// <param name="devices"></param>
        public static void SearchDevices(string keyword, string tag, List<Device> devices)
        {
            _lastSearchKeyword = keyword;
            _lastSearchTag = tag;
            var list = (from d in devices
                where _lastSearchKeyword == null || d.Id.ToString().Contains(keyword) || d.Sn.Contains(keyword) ||
                      d.Name.Contains(keyword) ||
                      d.Description.Contains(keyword)
                orderby d.Name
                select d).ToList();
            var list2 = PanelDevices.ToList();
            foreach (var device in list2)
            {
                if (!list.Contains(device))
                {
                    PanelDevices.Remove(device);
                }
            }

            foreach (var device in list)
            {
                if (!list2.Contains(device))
                {
                    PanelDevices.Add(device);
                }
            }

        }

        /// <summary>
        ///     获取设备连接信息
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static Device GetDevice(string sn)
        {
            var response = CallApi("get_device",
                new JObject {["sn"] = sn});
            if (!response.Result) Console.WriteLine(response.Message);

            return !response.Result ? null : PushDevice(response.Json);
        }

        /// <summary>
        /// 修改设备信息
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static Response SetDevice(string sn, string name, string desc, string[] tags)
        {
            var response = CallApi("set_device",
                new JObject {["sn"] = sn, ["name"] = name, ["desc"] = desc, ["tags"] = JArray.FromObject(tags)});
            return response;
        }

        /// <summary>
        ///     添加或更新Device
        /// </summary>
        /// <param name="json"></param>
        private static Device PushDevice(JToken json)
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
                    try
                    {
                        device.Tags = ((JArray) json["t_tags"]).ToObject<string[]>();
                    }
                    catch
                    {
                        device.Tags = new string[0];
                    }
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
                        Id = (int)json["t_id"],
                        Model = json["t_model"]?.ToString(),
                        Name = json["t_name"]?.ToString(),
                        Node = json["t_node"]?.ToString(),
                        Product = json["t_product"]?.ToString(),
                        Sn = json["t_sn"]?.ToString(),
                        Cpus = (int)json["t_cpus"],
                        Memory = (int)json["t_memory"],
                        RunningModules = new ObservableCollection<Module>()
                    };
                    try
                    {
                        device.Tags = ((JArray) json["t_tags"]).ToObject<string[]>();
                    }
                    catch
                    {
                        device.Tags = new string[0];
                    }

                    //初始化脚本引擎
                    device.ScriptEngine = new Script(device);
                    StartAction(() =>
                    {
                        try
                        {
                            using (var client = new WebClient())
                            {
                                var data = client.DownloadData("http://static.xky.com/screenshot/" + device.Sn +
                                                               ".jpg?x-oss-process=image/resize,h_100,w_52");
                                MainWindow.Dispatcher.Invoke(
                                    () => { device.ScreenShot = ByteToBitmapSource(data); });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });

                    //添加节点服务器
                    //StartAction(() => { PushNode(device.NodeSerial); });

                    if (device.Tags.Length == 0)
                    {
                        device.Tags = new[] {"未分组设备"};
                    }

                    foreach (var name in device.Tags)
                    {
                        AddTags(name, device);
                    }

                    AddTags("所有设备", device);

                    Tags = new ObservableCollection<Tag>(Tags.OrderByDescending(item => item.Count));


                    //用UI线程委托添加，防止报错
                    MainWindow.Dispatcher.Invoke(() =>
                    {
                        Devices.Add(device);
                        ParsePanelDevice(device, false);
                    });
                }

                return device;
            }
        }

        /// <summary>
        /// 标签、设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="device"></param>
        public static void AddTags(string name, Device device)
        {
            var tag = Tags.ToList().Find(p => p.Name == name);

            if (tag != null)
            {
                tag.Devices.Add(device);
                tag.Count++;
            }
            else
            {
                tag = new Tag
                {
                    Name = name,
                    Count = 1,
                    Devices = new List<Device>()
                };
                tag.Devices.Add(device);
                MainWindow.Dispatcher.Invoke(() => { Tags.Add(tag); });
            }
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="name"></param>
        /// <param name="device"></param>
        public static void RemoveTags(string name, Device device)
        {
            var tag = Tags.ToList().Find(p => p.Name == name);

            if (tag != null)
            {
                tag.Devices.Remove(device);
                tag.Count--;
            }
        }

        /// <summary>
        /// 解析最新设备决定是否放入面板
        /// </summary>
        /// <param name="device"></param>
        /// <param name="isRemove"></param>
        private static void ParsePanelDevice(Device device, bool isRemove)
        {
            if (_lastSearchKeyword == null || device.Id.ToString().Contains(_lastSearchKeyword) ||
                device.Sn.Contains(_lastSearchKeyword) ||
                device.Name.Contains(_lastSearchKeyword) || device.Description.Contains(_lastSearchKeyword) &&
                device.Tags.Contains(_lastSearchTag))
            {
                var find = PanelDevices.ToList().Find(p => p.Id == device.Id);
                if (isRemove)
                {
                    if (find == null) return;
                    PanelDevices.Remove(device);
                }
                else
                {
                    if (find != null) return;
                    PanelDevices.Add(device);
                }
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
                if (device != null)
                    MainWindow.Dispatcher.Invoke(() =>
                    {
                        foreach (var name in device.Tags)
                        {
                            RemoveTags(name, device);
                        }

                        RemoveTags("所有设备", device);


                        Devices.Remove(device);
                        ParsePanelDevice(device, true);
                    });
            }
        }


        /// <summary>
        /// 连接到节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="url"></param>
        /// <param name="hash"></param>
        private static void ConnectToNode(Node node, string url, string hash)
        {
            lock ("node_connect")
            {
                if (url == null || node.ConnectStatus > 0 && node.NodeUrl == url) return;

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
                    node.NodeSocket.On(Socket.Client.Socket.EventConnect, () =>
                    {
                        node.ConnectStatus = url.Contains("xxapi.org") ? 1 : 2;
                        Console.WriteLine("node Connected " + url);
                    });
                    node.NodeSocket.On(Socket.Client.Socket.EventDisconnect, () =>
                    {
                        node.ConnectStatus = 0;
                        Console.WriteLine("node Disconnected");
                    });
                    node.NodeSocket.On(Socket.Client.Socket.EventError, () => { Console.WriteLine("node ERROR"); });
                    node.NodeSocket.On("event", json => { Console.WriteLine(json); });
                    node.NodeSocket.On("tick", new MyListenerImpl((sn, json) =>
                    {
                        //Console.WriteLine(json);
                        var device = Devices.ToList().Find(p => p.Sn == sn.ToString());
                        if (device != null)
                        {
                            try
                            {
                                var model = JsonConvert.DeserializeObject<JObject>(json.ToString());
                                device.CpuUseage = Convert.ToInt32(model["cpu"].ToString());
                                device.MemoryUseage = Convert.ToInt32(model["memory"].ToString());
                                device.DiskUseage = Convert.ToInt32(model["disk"].ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }));
                    node.NodeSocket.On("img",
                        new MyListenerImpl((sn, data) =>
                        {
                            var imgdata = (byte[]) data;
                            //加入速率计数器
                            BitAverageNumber.Push(imgdata.Length);
                            var device = Devices.ToList().Find(p => p.Sn == (string) sn);
                            if (device != null && device.Sn != MirrorScreen.CurrentDevice?.Sn)
                            {
                                device.ScreenShot = ByteToBitmapSource((byte[]) data);
                            }
                        }));
                });
            }
        }

        #endregion

        #region 面板操作

        /// <summary>
        /// 弹出面板
        /// </summary>
        /// <param name="control"></param>
        public static void ShowDialogPanel(System.Windows.Controls.UserControl control)
        {
            ShowDialogPanelEvent?.Invoke(control);
        }

        /// <summary>
        /// 关闭弹出面板
        /// </summary>
        public static void CloseDialogPanel()
        {
            CloseDialogPanelEvent?.Invoke();
        }


        /// <summary>
        /// show toast
        /// </summary>
        /// <param name="toast"></param>
        /// <param name="color"></param>
        /// <param name="sound"></param>
        public static void ShowToast(string toast, Color color, string sound = null)
        {
            ShowToastEvent?.Invoke(toast,color,sound);
        }

        #endregion

        #region 全局事件

        /// <summary>
        /// 关闭弹出面板
        /// </summary>
        public static event OnCloseDialogPanel CloseDialogPanelEvent;

        /// <summary>
        /// 关闭弹出面板
        /// </summary>
        public delegate void OnCloseDialogPanel();


        /// <summary>
        /// 弹出面板
        /// </summary>
        public static event OnShowDialogPanel ShowDialogPanelEvent;

        /// <summary>
        /// 弹出面板
        /// </summary>
        public delegate void OnShowDialogPanel(System.Windows.Controls.UserControl control);

        /// <summary>
        /// 显示toast
        /// </summary>
        public static event OnShowToast ShowToastEvent;

        /// <summary>
        /// 显示toast
        /// </summary>
        public delegate void OnShowToast(string toast, Color color, string sound = null);

        #endregion

        #region  内部方法

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
                        PushDevice(json["device"]);
                    else
                        RemoveDevice(json["device"]);

                    break;
                }
                case "node_state":
                {
                    if (json["message"]?.ToString() == "online")
                    {
                        Console.WriteLine("节点上线：" + json["node"]["t_serial"].ToString());
                        var node = new Node
                        {
                            Serial = json["node"]["t_serial"]?.ToString(),
                            Name = json["node"]["t_name"]?.ToString(),
                            ConnectionHash = json["node"]["t_connection_hash"]?.ToString(),
                            Forward = json["node"]["t_forward"]?.ToString(),
                            Ip = json["node"]["t_ip"]?.ToString(),
                            Id = json["node"]["t_id"] == null ? 0 : Convert.ToInt32(json["node"]["t_id"]),
                            NodeUrl = json["node"]["t_nodeurl"]?.ToString()
                        };
                        //var oldnode= Nodes.ToList().Find(p => p.Serial == node.Serial);
                        //if (oldnode != null) {
                        //    oldnode = node;
                        //}
                        //else {
                        PushNode(node, false);
                        // }
                    }

                    else
                        RemoveNode(Convert.ToInt32(json["node"]["t_id"].ToString()));

                    break;
                }
                case "device_event":
                {
                    Console.WriteLine("收到事件");

                    break;
                }
                case "ime_status":
                {
                    Console.WriteLine("输入法状态改变事件:" + json["enable"]);
                    lock ("devices")
                    {
                        var device = Devices.ToList().Find(p => p.Sn == (string) json["sn"]);
                        if (device != null)
                        {
                            device.ImeEnable = (bool) json["enable"];
                        }
                    }

                    break;
                }
                default:
                {
                    //  Console.WriteLine("收到未处理的事件：" + json);

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

        #region 接口调用方法

        /// <summary>
        /// http调用接口
        /// </summary>
        /// <param name="api"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        private static Response Post(string api, JObject json)
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


        /// <summary>
        /// 调用平台接口
        /// </summary>
        /// <param name="api"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Response CallApi(string api, JObject data)
        {
            var response = new Response
            {
                Result = false,
                Message = "调用接口超时",
                Json = new JObject {["errcode"] = 1, ["msg"] = "调用接口超时"}
            };
            var count = 10000;

            if (_coreSocket == null || !CoreConnected)
                return new Response
                {
                    Result = false,
                    Message = "未连接核心服务器",
                    Json = new JObject {["errcode"] = 1, ["msg"] = "未连接核心服务器"}
                };

            _coreSocket.Emit("call", result =>
            {
                var jsonResult = (JObject) result;
                if (jsonResult == null || !jsonResult.ContainsKey("encrypt"))
                {
                    response = new Response
                    {
                        Result = false,
                        Message = "通讯结果无法解析",
                        Json = new JObject {["errcode"] = 1, ["msg"] = "通讯结果无法解析"}
                    };
                }
                else
                {
                    var resultJson =
                        JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()));
                    response = new Response
                    {
                        Result = resultJson["errcode"] != null && Convert.ToInt32(resultJson["errcode"]) == 0,
                        Message = resultJson["msg"]?.ToString(),
                        Json = JsonConvert.DeserializeObject<JObject>(Rsa.DecrypteRsa(jsonResult["encrypt"].ToString()))
                    };
                }

                //设置跳出循环条件
                count = 0;
            }, api, data);


            while (count > 0)
            {
                count -= 1;
                Thread.Sleep(1);
            }

            return response;
        }

        /// <summary>
        /// 调用节点接口
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="sn"></param>
        /// <param name="api"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Response CallNodeApi(string serial, string sn, string api, JArray args)
        {
            var node = Nodes.ToList().Find(p => p.Serial == serial);
            if (node == null)
            {
                return new Response
                {
                    Result = false,
                    Message = "节点服务器不存在",
                    Json = new JObject {["errcode"] = 1, ["msg"] = "节点服务器不存在"}
                };
            }

            if (node.ConnectStatus == 0)
            {
                return new Response
                {
                    Result = false,
                    Message = "节点服务器未连接",
                    Json = new JObject {["errcode"] = 1, ["msg"] = "节点服务器未连接"}
                };
            }


            var response = new Response
            {
                Result = false,
                Message = "调用接口超时",
                Json = new JObject {["errcode"] = 1, ["msg"] = "调用接口超时"}
            };
            var count = 20000;
            node.NodeSocket.Emit("call",
                result =>
                {
                    var resultJson = (JObject) result;
                    if (resultJson == null)
                    {
                        response = new Response
                        {
                            Result = false,
                            Message = "通讯结果无法解析",
                            Json = new JObject {["errcode"] = 1, ["msg"] = "通讯结果无法解析"}
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            Result = resultJson["errcode"] != null && Convert.ToInt32(resultJson["errcode"]) == 0,
                            Message = resultJson["msg"]?.ToString(),
                            Json = resultJson
                        };
                    }

                    //设置跳出循环条件
                    count = 0;
                }, new JObject
                {
                    ["sn"] = sn,
                    ["session"] = License.Session,
                    ["api"] = api,
                    ["args"] = args
                });

            while (count > 0)
            {
                count -= 1;
                Thread.Sleep(1);
            }

            return response;
        }

        /// <summary>
        /// 调用节点事件
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="sns"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Response CallNodeEvent(string serial, JArray sns, JObject json)
        {
            var node = Nodes.ToList().Find(p => p.Serial == serial);
            if (node == null)
            {
                return new Response
                {
                    Result = false,
                    Message = "节点服务器不存在",
                    Json = new JObject {["errcode"] = 1, ["msg"] = "节点服务器不存在"}
                };
            }

            if (node.ConnectStatus == 0)
            {
                return new Response
                {
                    Result = false,
                    Message = "节点服务器未连接",
                    Json = new JObject {["errcode"] = 1, ["msg"] = "节点服务器未连接"}
                };
            }


            var response = new Response
            {
                Result = true,
                Message = "指令已下发",
                Json = new JObject {["errcode"] = 1, ["msg"] = "指令已下发"}
            };
            node.NodeSocket.Emit("event", sns, json);
            return response;
        }

        #endregion
        #region 批量控制方法

        /// <summary>
        /// 根据标签发送批量控制命令
        /// </summary>
        /// <param name="json"></param>
        public static void CallBatchControlEnvent(JObject json)
        {
            if (BatchControlTag != null)
            {
                var nodeSerialList = (from d in BatchControlTag.Devices select d.NodeSerial).Distinct();
                foreach (string nodeSerial in nodeSerialList)
                {
                    Response res = Client.CallNodeEvent(nodeSerial, new JArray() { (from device in BatchControlTag.Devices.FindAll(d => d.NodeSerial == nodeSerial) select device.Sn).ToList() },json);
                    Console.WriteLine(res.Json.ToString());
                }
            }
        }
        #endregion
    }
}