using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XSocket.Engine.ComponentEmitter;
using XSocket.Engine.Modules;
using XSocket.Modules;
using XSocket.Parser;

namespace XSocket.Client
{
    public class Socket : Emitter
    {
        public static readonly string EventConnect = "connect";
        public static readonly string EventDisconnect = "disconnect";
        public static readonly string EventError = "error";
        public static readonly string EventMessage = "message";
        public static readonly string EventConnectError = Manager.EVENT_CONNECT_ERROR;
        public static readonly string EventConnectTimeout = Manager.EVENT_CONNECT_TIMEOUT;
        public static readonly string EventReconnect = Manager.EVENT_RECONNECT;
        public static readonly string EventReconnectError = Manager.EVENT_RECONNECT_ERROR;
        public static readonly string EventReconnectFailed = Manager.EVENT_RECONNECT_FAILED;
        public static readonly string EventReconnectAttempt = Manager.EVENT_RECONNECT_ATTEMPT;
        public static readonly string EventReconnecting = Manager.EVENT_RECONNECTING;

        private static readonly List<string> Events = new List<string>
        {
            EventConnect,
            EventConnectError,
            EventConnectTimeout,
            EventDisconnect,
            EventError,
            EventReconnect,
            EventReconnectAttempt,
            EventReconnectFailed,
            EventReconnectError,
            EventReconnecting
        };

        private readonly Manager _io;
        private readonly string _nsp;
        private ImmutableDictionary<int, IAck> _acks = ImmutableDictionary.Create<int, IAck>();

        private bool _connected;

        //private bool Disconnected = true;
        private int _ids;
        private ImmutableQueue<List<object>> _receiveBuffer = ImmutableQueue.Create<List<object>>();
        private ImmutableQueue<Packet> _sendBuffer = ImmutableQueue.Create<Packet>();
        private ImmutableQueue<On.IHandle> _subs;

        private object _lockObj=new object();

        public Socket(Manager io, string nsp)
        {
            _io = io;
            _nsp = nsp;
            SubEvents();
        }

        private void SubEvents()
        {
            var io = _io;
            _subs = ImmutableQueue.Create<On.IHandle>();
            _subs = _subs.Enqueue(Client.On.Create(io, Manager.EVENT_OPEN, new ListenerImpl(OnOpen)));
            _subs = _subs.Enqueue(Client.On.Create(io, Manager.EVENT_PACKET,
                new ListenerImpl(data => OnPacket((Packet) data))));
            _subs = _subs.Enqueue(Client.On.Create(io, Manager.EVENT_CLOSE,
                new ListenerImpl(data => OnClose((string) data))));
        }


        public Socket Open()
        {
            Task.Run(() =>
            {
                if (!_connected)
                {
                    _io.Open();
                    if (_io.ReadyState == Manager.ReadyStateEnum.OPEN) OnOpen();
                }
            });
            return this;
        }

        public Socket Connect()
        {
            return Open();
        }

        public Socket Send(params object[] args)
        {
            Emit(EventMessage, args);
            return this;
        }


        public override Emitter Emit(string eventString, params object[] arg)
        {
            var log = LogManager.GetLogger(Global.CallerName());

            if (Events.Contains(eventString))
            {
                base.Emit(eventString, arg);
                return this;
            }

            var args = new List<object> {eventString};
            args.AddRange(arg);

            var ack = args[args.Count - 1] as IAck;
            if (ack != null) args.RemoveAt(args.Count - 1);

            var jsonArgs = Parser.Packet.Args2JArray(args);

            var parserType = HasBinaryData.HasBinary(jsonArgs) ? Parser.Parser.BINARY_EVENT : Parser.Parser.EVENT;
            var packet = new Packet(parserType, jsonArgs);

            if (ack != null)
            {
                log.Info($"emitting packet with ack id {_ids}");

                lock (_lockObj)
                {
                    _acks = _acks.Add(_ids, ack);
                }

                packet.Id = _ids++;
            }

            if (_connected)
                Packet(packet);
            else
                _sendBuffer = _sendBuffer.Enqueue(packet);

            return this;
        }


        public Emitter Emit(string eventString, IAck ack, params object[] args)
        {
            var log = LogManager.GetLogger(Global.CallerName());

            if (Events.Contains(eventString))
            {
                base.Emit(eventString, args);
                return this;
            }

            var _args = new List<object> {eventString};
            _args.AddRange(args);

            var jsonArgs = Parser.Packet.Args2JArray(_args);

            var parserType = HasBinaryData.HasBinary(jsonArgs) ? Parser.Parser.BINARY_EVENT : Parser.Parser.EVENT;
            var packet = new Packet(parserType, jsonArgs);

            log.Info($"emitting packet with ack id {_ids}");
            lock (_lockObj)
            {
                _acks = _acks.Add(_ids, ack);
            }

            packet.Id = _ids++;

            if (_connected)
                Packet(packet);
            else
                _sendBuffer = _sendBuffer.Enqueue(packet);

            return this;
        }

        public Emitter Emit(string eventString, Action ack, params object[] args)
        {
            return Emit(eventString, new AckImpl(ack), args);
        }

        public Emitter Emit(string eventString, Action<object> ack, params object[] args)
        {
            return Emit(eventString, new AckImpl(ack), args);
        }

        public Emitter Emit(string eventString, Action<object, object> ack, params object[] args)
        {
            return Emit(eventString, new AckImpl(ack), args);
        }

        public Emitter Emit(string eventString, Action<object, object, object> ack, params object[] args)
        {
            return Emit(eventString, new AckImpl(ack), args);
        }

        public Emitter Emit(string eventString, Action<object, object, object, object> ack, params object[] args)
        {
            return Emit(eventString, new AckImpl(ack), args);
        }

        public void Packet(Packet packet)
        {
            packet.Nsp = _nsp;
            _io.Packet(packet);
        }

        private void OnOpen()
        {
            //var log = LogManager.GetLogger(Global.CallerName());
            if (_nsp != "/") Packet(new Packet(Parser.Parser.CONNECT));
        }

        private void OnClose(string reason)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info($"close ({reason})");
            _connected = false;
            Emit(EventDisconnect, reason);
        }

        private void OnPacket(Packet packet)
        {
            if (_nsp != packet.Nsp) return;

            switch (packet.Type)
            {
                case Parser.Parser.CONNECT:
                    OnConnect();
                    break;

                case Parser.Parser.EVENT:
                    OnEvent(packet);
                    break;

                case Parser.Parser.BINARY_EVENT:
                    OnEvent(packet);
                    break;

                case Parser.Parser.ACK:
                    OnAck(packet);
                    break;

                case Parser.Parser.BINARY_ACK:
                    OnAck(packet);
                    break;

                case Parser.Parser.DISCONNECT:
                    OnDisconnect();
                    break;

                case Parser.Parser.ERROR:
                    Emit(EventError, packet.Data);
                    break;
            }
        }


        private void OnEvent(Packet packet)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            //var jarr =(string) ((JValue) packet.Data).Value;
            //var job = JToken.Parse(jarr);


            //var arr = job.ToArray();

            //var arg = job.Select(token => token.Value<string>()).Cast<object>().ToList();
            var args = packet.GetDataAsList();


            log.Info($"emitting event {args}");
            if (packet.Id >= 0)
            {
                log.Info("attaching ack callback to event");
                args.Add(new AckImp(this, packet.Id));
            }

            if (_connected)
            {
                var eventString = (string) args[0];
                args.Remove(args[0]);
                base.Emit(eventString, args.ToArray());
            }
            else
            {
                _receiveBuffer = _receiveBuffer.Enqueue(args);
            }
        }

        private void OnAck(Packet packet)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info($"calling ack {packet.Id} with {packet.Data}");
            IAck fn;
            lock (_lockObj)
            {
                fn = _acks[packet.Id];
                _acks = _acks.Remove(packet.Id);
            }

            var args = packet.GetDataAsList();

            fn.Call(args.ToArray());
        }


        private void OnConnect()
        {
            _connected = true;
            //Disconnected = false;
            Emit(EventConnect);
            EmitBuffered();
        }

        private void EmitBuffered()
        {
            while (_receiveBuffer.Count() > 0)
            {
                List<object> data;
                _receiveBuffer = _receiveBuffer.Dequeue(out data);
                var eventString = (string) data[0];
                base.Emit(eventString, data.ToArray());
            }

            _receiveBuffer = _receiveBuffer.Clear();


            while (_sendBuffer.Count() > 0)
            {
                Packet packet;
                _sendBuffer = _sendBuffer.Dequeue(out packet);
                Packet(packet);
            }

            _sendBuffer = _sendBuffer.Clear();
        }


        private void OnDisconnect()
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info($"server disconnect ({_nsp})");
            Destroy();
            OnClose("io server disconnect");
        }

        private void Destroy()
        {
            foreach (var sub in _subs) sub.Destroy();

            _subs = _subs.Clear();

            _io.Destroy(this);
        }

        public Socket Close()
        {
            var log = LogManager.GetLogger(Global.CallerName());

            if (_connected)
            {
                log.Info($"performing disconnect ({_nsp})");
                Packet(new Packet(Parser.Parser.DISCONNECT));
            }

            Destroy();

            if (_connected) OnClose("io client disconnect");

            return this;
        }

        public Socket Disconnect()
        {
            return Close();
        }

        public Manager Io()
        {
            return _io;
        }

        private static IEnumerable<object> ToArray(JArray array)
        {
            var length = array.Count;
            var data = new object[length];
            for (var i = 0; i < length; i++)
            {
                object v;
                try
                {
                    v = array[i];
                }
                catch (Exception)
                {
                    v = null;
                }

                data[i] = v;
            }

            return data;
        }

        private class AckImp : IAck
        {
            private readonly int _id;
            private readonly bool[] _sent = {false};
            private readonly Socket _socket;

            public AckImp(Socket socket, int id)
            {
                _socket = socket;
                _id = id;
            }

            public void Call(params object[] args)
            {
                if (_sent[0]) return;

                _sent[0] = true;
                var log = LogManager.GetLogger(Global.CallerName());
                var jsonArgs = Parser.Packet.Args2JArray(args);
                log.Info($"sending ack {(args.Length != 0 ? jsonArgs.ToString() : "null")}");

                var parserType = HasBinaryData.HasBinary(args) ? Parser.Parser.BINARY_ACK : Parser.Parser.ACK;
                var packet = new Packet(parserType, jsonArgs);
                packet.Id = _id;
                _socket.Packet(packet);
            }
        }
    }
}