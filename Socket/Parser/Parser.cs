using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using XSocket.Client;
using XSocket.Engine.ComponentEmitter;
using XSocket.Engine.Modules;

namespace XSocket.Parser
{
    public class Parser
    {
        public const int CONNECT = 0;
        public const int DISCONNECT = 1;
        public const int EVENT = 2;
        public const int ACK = 3;
        public const int ERROR = 4;
        public const int BINARY_EVENT = 5;
        public const int BINARY_ACK = 6;
        public const int protocol = 4;


        /// <summary>
        ///     Packet types
        /// </summary>
        public static List<string> types = new List<string>
        {
            "CONNECT",
            "DISCONNECT",
            "EVENT",
            "BINARY_EVENT",
            "ACK",
            "BINARY_ACK",
            "ERROR"
        };

        private static readonly Packet ErrorPacket = new Packet(ERROR, "parser error");

        private Parser()
        {
        }

        public class Encoder
        {
            public void Encode(Packet obj, ICallback callback)
            {
                var log = LogManager.GetLogger(Global.CallerName());
                log.Info(string.Format("encoding packet {0}", obj));

                if (BINARY_EVENT == obj.Type || BINARY_ACK == obj.Type)
                {
                    EncodeAsBinary(obj, callback);
                }
                else
                {
                    var encoding = EncodeAsString(obj);
                    callback.Call(new object[] {encoding});
                }
            }

            private string EncodeAsString(Packet obj)
            {
                var str = new StringBuilder();
                var nsp = false;

                str.Append(obj.Type);

                if (BINARY_EVENT == obj.Type || BINARY_ACK == obj.Type)
                {
                    str.Append(obj.Attachments);
                    str.Append("-");
                }

                if (!string.IsNullOrEmpty(obj.Nsp) && !"/".Equals(obj.Nsp))
                {
                    nsp = true;
                    str.Append(obj.Nsp);
                }

                if (obj.Id >= 0)
                {
                    if (nsp)
                    {
                        str.Append(",");
                        nsp = false;
                    }

                    str.Append(obj.Id);
                }

                if (obj.Data != null)
                {
                    if (nsp) str.Append(",");
                    if ((obj.Data as JToken).HasValues) str.Append(obj.Data);
                }

                var log = LogManager.GetLogger(Global.CallerName());
                log.Info(string.Format("encoded {0} as {1}", obj, str));
                return str.ToString();
            }

            private void EncodeAsBinary(Packet obj, ICallback callback)
            {
                var deconstruction = Binary.DeconstructPacket(obj);
                var pack = EncodeAsString(deconstruction.Packet);
                var buffers = new List<object>();
                foreach (var item in deconstruction.Buffers) buffers.Add(item);

                buffers.Insert(0, pack);
                callback.Call(buffers.ToArray());
            }

            public interface ICallback
            {
                void Call(object[] data);
            }

            public class CallbackImp : ICallback
            {
                private readonly Action<object[]> Fn;

                public CallbackImp(Action<object[]> fn)
                {
                    Fn = fn;
                }

                public void Call(object[] data)
                {
                    Fn(data);
                }
            }
        }


        public class Decoder : Emitter
        {
            public const string EVENT_DECODED = "decoded";

            /*package*/
            public BinaryReconstructor Reconstructor;

            public void Add(string obj)
            {
                var packet = decodeString(obj);
                if (packet.Type == BINARY_EVENT || packet.Type == BINARY_ACK)
                {
                    Reconstructor = new BinaryReconstructor(packet);

                    if (Reconstructor.reconPack.Attachments == 0) Emit(EVENT_DECODED, packet);
                }
                else
                {
                    Emit(EVENT_DECODED, packet);
                }
            }


            public void Add(byte[] obj)
            {
                if (Reconstructor == null)
                    throw new Exception("got binary data when not reconstructing a packet");

                var packet = Reconstructor.TakeBinaryData(obj);
                if (packet != null)
                {
                    Reconstructor = null;
                    Emit(EVENT_DECODED, packet);
                }
            }

            private Packet decodeString(string str)
            {
                var p = new Packet();
                var i = 0;

                p.Type = int.Parse(str.Substring(0, 1));
                if (p.Type < 0 || p.Type > types.Count - 1) return ErrorPacket;

                if (BINARY_EVENT == p.Type || BINARY_ACK == p.Type)
                {
                    var attachments = new StringBuilder();
                    while (str.Substring(++i, 1) != "-") attachments.Append(str.Substring(i, 1));
                    p.Attachments = int.Parse(attachments.ToString());
                }

                if (str.Length > i + 1 && "/" == str.Substring(i + 1, 1))
                {
                    var nsp = new StringBuilder();
                    while (true)
                    {
                        ++i;
                        var c = str.Substring(i, 1);
                        if ("," == c) break;
                        nsp.Append(c);
                        if (i + 1 == str.Length) break;
                    }

                    p.Nsp = nsp.ToString();
                }
                else
                {
                    p.Nsp = "/";
                }

                var next = i + 1 >= str.Length ? null : str.Substring(i + 1, 1);

                int unused;
                if (null != next && int.TryParse(next, out unused))
                {
                    var id = new StringBuilder();
                    while (true)
                    {
                        ++i;
                        var c = str.Substring(i, 1);

                        if (!int.TryParse(c, out unused))
                        {
                            --i;
                            break;
                        }

                        id.Append(c);
                        if (i + 1 >= str.Length) break;
                    }

                    p.Id = int.Parse(id.ToString());
                }


                if (i++ < str.Length)
                    try
                    {
                        var t = str.Substring(i);
                        p.Data = new JValue(t);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // do nothing
                    }
                    catch (Exception)
                    {
                        return ErrorPacket;
                    }

                var log = LogManager.GetLogger(Global.CallerName());
                log.Info(string.Format("decoded {0} as {1}", str, p));
                return p;
            }

            public void Destroy()
            {
                if (Reconstructor != null) Reconstructor.FinishReconstruction();
            }
        }

        /*package*/
        public class BinaryReconstructor
        {
            /*package*/
            public List<byte[]> Buffers;

            public Packet reconPack;

            public BinaryReconstructor(Packet packet)
            {
                reconPack = packet;
                Buffers = new List<byte[]>();
            }

            public Packet TakeBinaryData(byte[] binData)
            {
                Buffers.Add(binData);
                if (Buffers.Count == reconPack.Attachments)
                {
                    var packet = Binary.ReconstructPacket(reconPack,
                        Buffers.ToArray());
                    FinishReconstruction();
                    return packet;
                }

                return null;
            }

            public void FinishReconstruction()
            {
                reconPack = null;
                Buffers = new List<byte[]>();
            }
        }
    }
}