using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

namespace Xky.Core
{
    public class MirrorClient
    {
        internal readonly H264Decoder Decoder = new H264Decoder();
  
        public MirrorScreen MirrorScreen;


        public void Connect(string sn, string session)
        {
//            Console.WriteLine("正在获取设备" + sn + "的连接信息..");
//            MirrorScreen?.AddLabel("正在获取设备" + sn + "的连接信息..", Colors.White);
//            var device = Client.Post("get_device", new JObject {["sn"] = sn, ["session"] = session});
//            if (device["errcode"]?.ToString() != "0") throw new Exception("获取设备状态出错" + device["msg"]);
//
//            if (!device.ContainsKey("t_nodeurl") || !device["t_nodeurl"].ToString().StartsWith("http"))
//                throw new Exception("该设备没有设置P2P转发模式");
//
//            _socket?.Disconnect();
//            Decoder.Firstpacket = true;
//            var options = new IO.Options
//            {
//                IgnoreServerCertificateValidation = true,
//                AutoConnect = true,
//                ForceNew = true,
//                Query = new Dictionary<string, string>
//                {
//                    {"sn", device["t_sn"].ToString()},
//                    {"action", "mirror"},
//                    {"v2", "true"},
//                    {"hash", device["t_connection_hash"].ToString()}
//                },
//                Path = "/xky",
//                Transports = ImmutableList.Create("websocket")
//            };
//            MirrorScreen?.AddLabel("正在连接..", Colors.White);
//            _socket = IO.Socket(device["t_nodeurl"].ToString(), options);
//            _socket.On(Socket.EVENT_CONNECT, () => { Console.WriteLine("Connected"); });
//            _socket.On(Socket.EVENT_DISCONNECT, () => { Console.WriteLine("Disconnected"); });
//            _socket.On(Socket.EVENT_ERROR, () => { Console.WriteLine("ERROR"); });
//            _socket.On("event", json => { Console.WriteLine(json); });
//            _socket.On("h264", data => { Decoder?.Decode((byte[]) data); });
        }



        public void EmitEvent(JObject jObject)
        {
           // _socket?.Emit("event", jObject);
        }
    }
}