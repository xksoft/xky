using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Security.Authentication;
using System.Threading.Tasks;
using XSocket.Engine.Client.Transports;
using XSocket.Engine.ComponentEmitter;
using XSocket.Engine.Modules;
using XSocket.Engine.Parser;
using XSocket.Engine.Thread;

namespace XSocket.Engine.Client
{
    public class Socket : Emitter
    {
        public static readonly string EventOpen = "open";
        public static readonly string EventClose = "close";
        public static readonly string EventPacket = "packet";
        public static readonly string EventDrain = "drain";
        public static readonly string EventError = "error";
        public static readonly string EventData = "data";
        public static readonly string EventMessage = "message";
        public static readonly string EventUpgradeError = "upgradeError";
        public static readonly string EventFlush = "flush";
        public static readonly string EventHandshake = "handshake";
        public static readonly string EventUpgrading = "upgrading";
        public static readonly string EventUpgrade = "upgrade";
        public static readonly string EventPacketCreate = "packetCreate";
        public static readonly string EventHeartbeat = "heartbeat";
        public static readonly string EventTransport = "transport";

        public static readonly int Protocol = Parser.Parser.Protocol;

        public static bool PriorWebsocketSuccess;
        private const bool Agent = false;
        private readonly Dictionary<string, string> _cookies;
        private const bool ForceBase64 = false;
        private const bool ForceJsonp = false;
        private readonly string _hostname;
        private readonly string _path;
        private readonly int _policyPort;
        private readonly int _port;
        private readonly Dictionary<string, string> _query;
        private readonly bool _rememberUpgrade;


        private readonly bool _secure;
        private readonly SslProtocols _sslProtocols;
        private readonly string _timestampParam;

        private readonly bool _timestampRequests;
        private readonly ImmutableList<string> _transports;
        private readonly bool _upgrade;

        private int _errorCount;
        private ImmutableList<Action> _callbackBuffer = ImmutableList<Action>.Empty;

        public Dictionary<string, string> ExtraHeaders;
        public string Id;
        private long _pingInterval;
        private EasyTimer _pingIntervalTimer;
        private long _pingTimeout;
        private EasyTimer _pingTimeoutTimer;
        private int _prevBufferLen;

        private ReadyStateEnum _readyState;

        /*package*/
        public Transport Transport;
        private ImmutableList<string> _upgrades;
        private bool _upgrading;
        private ImmutableList<Packet> _writeBuffer = ImmutableList<Packet>.Empty;


        //public static void SetupLog4Net()
        //{
        //    var hierarchy = (Hierarchy)LogManager.GetRepository();
        //    hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

        //    var fileAppender = new FileAppender();
        //    fileAppender.AppendToFile = true;
        //    fileAppender.LockingModel = new FileAppender.MinimalLock();
        //    fileAppender.File = "EngineIoClientDotNet.log";
        //    var pl = new PatternLayout();
        //    pl.ConversionPattern = "%d [%2%t] %-5p [%-10c]   %m%n";
        //    pl.ActivateOptions();
        //    fileAppender.Layout = pl;
        //    fileAppender.ActivateOptions();
        //    BasicConfigurator.Configure(fileAppender);
        //}

        public Socket()
            : this(new Options())
        {
        }

        public Socket(string uri)
            : this(uri, null)
        {
        }

        public Socket(string uri, Options options)
            : this(uri == null ? null : String2Uri(uri), options)
        {
        }

        public Socket(Uri uri, Options options)
            : this(uri == null ? options : Options.FromUri(uri, options))
        {
        }


        public Socket(Options options)
        {
            if (options.Host != null)
            {
                var pieces = options.Host.Split(':');
                options.Hostname = pieces[0];
                if (pieces.Length > 1) options.Port = int.Parse(pieces[pieces.Length - 1]);
            }

            _secure = options.Secure;
            _sslProtocols = options.SslProtocols;
            _hostname = options.Hostname;
            _port = options.Port;
            _query = options.QueryString != null
                ? ParseQS.Decode(options.QueryString)
                : new Dictionary<string, string>();

            if (options.Query != null)
                foreach (var item in options.Query)
                    _query.Add(item.Key, item.Value);


            _upgrade = options.Upgrade;
            _path = (options.Path ?? "/engine.io").Replace("/$", "") + "/";
            _timestampParam = options.TimestampParam ?? "t";
            _timestampRequests = options.TimestampRequests;
            _transports = options.Transports ?? ImmutableList<string>.Empty.Add(Polling.NAME).Add(WebSocket.NAME);
            _policyPort = options.PolicyPort != 0 ? options.PolicyPort : 843;
            _rememberUpgrade = options.RememberUpgrade;
            _cookies = options.Cookies;
            if (options.IgnoreServerCertificateValidation) ServerCertificate.IgnoreServerCertificateValidation();
            ExtraHeaders = options.ExtraHeaders;
        }

        private static Uri String2Uri(string uri)
        {
            if (uri.StartsWith("http") || uri.StartsWith("ws"))
                return new Uri(uri);
            return new Uri("http://" + uri);
        }

        public Socket Open()
        {
            string transportName;
            if (_rememberUpgrade && PriorWebsocketSuccess && _transports.Contains(WebSocket.NAME))
                transportName = WebSocket.NAME;
            else
                transportName = _transports[0];
            _readyState = ReadyStateEnum.Opening;
            var transport = CreateTransport(transportName);
            SetTransport(transport);
//            EventTasks.Exec((n) =>
            Task.Run(() =>
            {
                var log2 = LogManager.GetLogger(Global.CallerName());
                log2.Info("Task.Run Open start");
                transport.Open();
                log2.Info("Task.Run Open finish");
            });
            return this;
        }

        private Transport CreateTransport(string name)
        {
            var query = new Dictionary<string, string>(_query);
            query.Add("EIO", Parser.Parser.Protocol.ToString());
            query.Add("transport", name);
            if (Id != null) query.Add("sid", Id);
            var options = new Transport.Options
            {
                Hostname = _hostname,
                Port = _port,
                Secure = _secure,
                SslProtocols = _sslProtocols,
                Path = _path,
                Query = query,
                TimestampRequests = _timestampRequests,
                TimestampParam = _timestampParam,
                PolicyPort = _policyPort,
                Socket = this,
                Agent = Agent,
                ForceBase64 = ForceBase64,
                ForceJsonp = ForceJsonp,
                Cookies = _cookies,
                ExtraHeaders = ExtraHeaders
            };

            if (name == WebSocket.NAME)
                return new WebSocket(options);
            if (name == Polling.NAME) return new PollingXHR(options);

            throw new EngineIOException("CreateTransport failed");
        }

        private void SetTransport(Transport transport)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info(string.Format("SetTransport setting transport '{0}'", transport.Name));

            if (Transport != null)
            {
                log.Info(string.Format("SetTransport clearing existing transport '{0}'", transport.Name));
                Transport.Off();
            }

            Transport = transport;

            Emit(EventTransport, transport);

            transport.On(EventDrain, new EventDrainListener(this));
            transport.On(EventPacket, new EventPacketListener(this));
            transport.On(EventError, new EventErrorListener(this));
            transport.On(EventClose, new EventCloseListener(this));
        }


        internal void OnDrain()
        {
            //var log = LogManager.GetLogger(Global.CallerName());
            //log.Info(string.Format("OnDrain1 PrevBufferLen={0} WriteBuffer.Count={1}", PrevBufferLen, WriteBuffer.Count));

            for (var i = 0; i < _prevBufferLen; i++)
                try
                {
                    var callback = _callbackBuffer[i];
                    if (callback != null) callback();
                }
                catch (ArgumentOutOfRangeException)
                {
                    _writeBuffer = _writeBuffer.Clear();
                    _callbackBuffer = _callbackBuffer.Clear();
                    _prevBufferLen = 0;
                }
            //log.Info(string.Format("OnDrain2 PrevBufferLen={0} WriteBuffer.Count={1}", PrevBufferLen, WriteBuffer.Count));


            try
            {
                _writeBuffer = _writeBuffer.RemoveRange(0, _prevBufferLen);
                _callbackBuffer = _callbackBuffer.RemoveRange(0, _prevBufferLen);
            }
            catch (Exception)
            {
                _writeBuffer = _writeBuffer.Clear();
                _callbackBuffer = _callbackBuffer.Clear();
            }


            _prevBufferLen = 0;
            //log.Info(string.Format("OnDrain3 PrevBufferLen={0} WriteBuffer.Count={1}", PrevBufferLen, WriteBuffer.Count));

            if (_writeBuffer.Count == 0)
                Emit(EventDrain);
            else
                Flush();
        }

        private bool Flush()
        {
            var log = LogManager.GetLogger(Global.CallerName());

            log.Info(string.Format("ReadyState={0} Transport.Writeable={1} Upgrading={2} WriteBuffer.Count={3}",
                _readyState, Transport.Writable, _upgrading, _writeBuffer.Count));
            if (_readyState != ReadyStateEnum.Closed && _readyState == ReadyStateEnum.Open && Transport.Writable &&
                !_upgrading && _writeBuffer.Count != 0)
            {
                log.Info(string.Format("Flush {0} packets in socket", _writeBuffer.Count));
                _prevBufferLen = _writeBuffer.Count;
                Transport.Send(_writeBuffer);
                Emit(EventFlush);
                return true;
            }

            log.Info("Flush Not Send");
            return false;
        }

        public void OnPacket(Packet packet)
        {
            var log = LogManager.GetLogger(Global.CallerName());


            if (_readyState == ReadyStateEnum.Opening || _readyState == ReadyStateEnum.Open)
            {
                log.Info(string.Format("socket received: type '{0}', data '{1}'", packet.Type, packet.Data));

                Emit(EventPacket, packet);
                Emit(EventHeartbeat);

                if (packet.Type == Packet.OPEN)
                {
                    OnHandshake(new HandshakeData((string) packet.Data));
                }
                else if (packet.Type == Packet.PONG)
                {
                    SetPing();
                }
                else if (packet.Type == Packet.ERROR)
                {
                    var err = new EngineIOException("server error")
                    {
                        code = packet.Data
                    };
                    Emit(EventError, err);
                }
                else if (packet.Type == Packet.MESSAGE)
                {
                    Emit(EventData, packet.Data);
                    Emit(EventMessage, packet.Data);
                }
            }
            else
            {
                log.Info(string.Format("OnPacket packet received with socket readyState '{0}'", _readyState));
            }
        }

        private void OnHandshake(HandshakeData handshakeData)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info(nameof(OnHandshake));
            Emit(EventHandshake, handshakeData);
            Id = handshakeData.Sid;
            Transport.Query.Add("sid", handshakeData.Sid);
            _upgrades = FilterUpgrades(handshakeData.Upgrades);
            _pingInterval = handshakeData.PingInterval;
            _pingTimeout = handshakeData.PingTimeout;
            OnOpen();
            // In case open handler closes socket
            if (ReadyStateEnum.Closed == _readyState) return;
            SetPing();

            Off(EventHeartbeat, new OnHeartbeatAsListener(this));
            On(EventHeartbeat, new OnHeartbeatAsListener(this));
        }


        private void SetPing()
        {
            //var log = LogManager.GetLogger(Global.CallerName());

            if (_pingIntervalTimer != null) _pingIntervalTimer.Stop();
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info(string.Format("writing ping packet - expecting pong within {0}ms", _pingTimeout));

            _pingIntervalTimer = EasyTimer.SetTimeout(() =>
            {
                var log2 = LogManager.GetLogger(Global.CallerName());
                log2.Info("EasyTimer SetPing start");

                if (_upgrading)
                {
                    // skip this ping during upgrade
                    SetPing();
                    log2.Info("skipping Ping during upgrade");
                }
                else if (_readyState == ReadyStateEnum.Open)
                {
                    Ping();
                    OnHeartbeat(_pingTimeout);
                    log2.Info("EasyTimer SetPing finish");
                }
            }, (int) _pingInterval);
        }

        private void Ping()
        {
            //Send("primus::ping::" + GetJavaTime());
            SendPacket(Packet.PING);
        }

        //private static string GetJavaTime()
        //{
        //    var st = new DateTime(1970, 1, 1);
        //    var t = (DateTime.Now.ToUniversalTime() - st);
        //    var returnstring = t.TotalMilliseconds.ToString();
        //    returnstring = returnstring.Replace(".", "-");
        //    return returnstring;
        //}

        public void Write(string msg, Action fn = null)
        {
            Send(msg, fn);
        }

        public void Write(byte[] msg, Action fn = null)
        {
            Send(msg, fn);
        }

        public void Send(string msg, Action fn = null)
        {
            SendPacket(Packet.MESSAGE, msg, fn);
        }

        public void Send(byte[] msg, Action fn = null)
        {
            SendPacket(Packet.MESSAGE, msg, fn);
        }


        private void SendPacket(string type)
        {
            SendPacket(new Packet(type), null);
        }

        private void SendPacket(string type, string data, Action fn)
        {
            SendPacket(new Packet(type, data), fn);
        }

        private void SendPacket(string type, byte[] data, Action fn)
        {
            SendPacket(new Packet(type, data), fn);
        }

        private void SendPacket(Packet packet, Action fn)
        {
            if (fn == null) fn = () => { };

            if (_upgrading) WaitForUpgrade().Wait();

            Emit(EventPacketCreate, packet);
            //var log = LogManager.GetLogger(Global.CallerName());
            //log.Info(string.Format("SendPacket WriteBuffer.Add(packet) packet ={0}",packet.Type));
            _writeBuffer = _writeBuffer.Add(packet);
            _callbackBuffer = _callbackBuffer.Add(fn);
            Flush();
        }

        //private async Task WaitForUpgradeAsync()
        //{
        //    const int TIMEOUT = 1000;

        //    await Task.Yield();
        //    if (!SpinWait.SpinUntil(() => !Upgrading, TIMEOUT))
        //    {
        //        var log = LogManager.GetLogger(Global.CallerName());
        //        log.Info("Wait for upgrade timeout");
        //    }
        //}

        private Task WaitForUpgrade()
        {
            var log = LogManager.GetLogger(Global.CallerName());

            var tcs = new TaskCompletionSource<object>();
            const int timeout = 1000;
            var sw = new Stopwatch();

            try
            {
                sw.Start();
                while (_upgrading)
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        log.Info("Wait for upgrade timeout");
                        break;
                    }

                tcs.SetResult(null);
            }
            finally
            {
                sw.Stop();
            }

            return tcs.Task;
        }

        private void OnOpen()
        {
            var log = LogManager.GetLogger(Global.CallerName());

            //log.Info("socket open before call to flush()");
            _readyState = ReadyStateEnum.Open;
            PriorWebsocketSuccess = WebSocket.NAME == Transport.Name;

            Flush();
            Emit(EventOpen);


            if (_readyState == ReadyStateEnum.Open && _upgrade && Transport is Polling)
                //if (ReadyState == ReadyStateEnum.OPEN && Upgrade && this.Transport)
            {
                log.Info("OnOpen starting upgrade probes");
                _errorCount = 0;
                foreach (var upgrade in _upgrades) Probe(upgrade);
            }
        }

        private void Probe(string name)
        {
            var log = LogManager.GetLogger(Global.CallerName());

            log.Info(string.Format("Probe probing transport '{0}'", name));

            PriorWebsocketSuccess = false;

            var transport = CreateTransport(name);
            var parameters = new ProbeParameters
            {
                Transport = ImmutableList<Transport>.Empty.Add(transport),
                Failed = ImmutableList<bool>.Empty.Add(false),
                Cleanup = ImmutableList<Action>.Empty,
                Socket = this
            };

            var onTransportOpen = new OnTransportOpenListener(parameters);
            var freezeTransport = new FreezeTransportListener(parameters);

            // Handle any error that happens while probing
            var onError = new ProbingOnErrorListener(this, parameters.Transport, freezeTransport);
            var onTransportClose = new ProbingOnTransportCloseListener(onError);

            // When the socket is closed while we're probing
            var onClose = new ProbingOnCloseListener(onError);

            var onUpgrade = new ProbingOnUpgradeListener(freezeTransport, parameters.Transport);


            parameters.Cleanup = parameters.Cleanup.Add(() =>
            {
                if (parameters.Transport.Count < 1) return;

                parameters.Transport[0].Off(Transport.EventOpen, onTransportOpen);
                parameters.Transport[0].Off(Transport.EventError, onError);
                parameters.Transport[0].Off(Transport.EventClose, onTransportClose);
                Off(EventClose, onClose);
                Off(EventUpgrading, onUpgrade);
            });

            parameters.Transport[0].Once(Transport.EventOpen, onTransportOpen);
            parameters.Transport[0].Once(Transport.EventError, onError);
            parameters.Transport[0].Once(Transport.EventClose, onTransportClose);

            Once(EventClose, onClose);
            Once(EventUpgrading, onUpgrade);

            parameters.Transport[0].Open();
        }

        public Socket Close()
        {
            if (_readyState == ReadyStateEnum.Opening || _readyState == ReadyStateEnum.Open)
            {
                var log = LogManager.GetLogger(Global.CallerName());
                log.Info("Start");
                OnClose("forced close");

                log.Info("socket closing - telling transport to close");
                Transport.Close();
            }

            return this;
        }

        private void OnClose(string reason, Exception desc = null)
        {
            if (_readyState == ReadyStateEnum.Opening || _readyState == ReadyStateEnum.Open)
            {
                var log = LogManager.GetLogger(Global.CallerName());

                log.Info(string.Format("OnClose socket close with reason: {0}", reason));

                // clear timers
                if (_pingIntervalTimer != null) _pingIntervalTimer.Stop();
                if (_pingTimeoutTimer != null) _pingTimeoutTimer.Stop();


                //WriteBuffer = WriteBuffer.Clear();
                //CallbackBuffer = CallbackBuffer.Clear();
                //PrevBufferLen = 0;

                EasyTimer.SetTimeout(() =>
                {
                    _writeBuffer = ImmutableList<Packet>.Empty;
                    _callbackBuffer = ImmutableList<Action>.Empty;
                    _prevBufferLen = 0;
                }, 1);


                if (Transport != null)
                {
                    // stop event from firing again for transport
                    Transport.Off(EventClose);

                    // ensure transport won't stay open
                    Transport.Close();

                    // ignore further transport communication
                    Transport.Off();
                }

                // set ready state
                _readyState = ReadyStateEnum.Closed;

                // clear session id
                Id = null;

                // emit close events
                Emit(EventClose, reason, desc);
            }
        }

        public ImmutableList<string> FilterUpgrades(IEnumerable<string> upgrades)
        {
            var filterUpgrades = ImmutableList<string>.Empty;
            foreach (var upgrade in upgrades)
                if (_transports.Contains(upgrade))
                    filterUpgrades = filterUpgrades.Add(upgrade);
            return filterUpgrades;
        }


        internal void OnHeartbeat(long timeout)
        {
            if (_pingTimeoutTimer != null)
            {
                _pingTimeoutTimer.Stop();
                _pingTimeoutTimer = null;
            }

            if (timeout <= 0) timeout = _pingInterval + _pingTimeout;

            _pingTimeoutTimer = EasyTimer.SetTimeout(() =>
            {
                var log2 = LogManager.GetLogger(Global.CallerName());
                log2.Info("EasyTimer OnHeartbeat start");
                if (_readyState == ReadyStateEnum.Closed)
                {
                    log2.Info("EasyTimer OnHeartbeat ReadyState == ReadyStateEnum.CLOSED finish");
                    return;
                }

                OnClose("ping timeout");
                log2.Info("EasyTimer OnHeartbeat finish");
            }, (int) timeout);
        }

        internal void OnError(Exception exception)
        {
            var log = LogManager.GetLogger(Global.CallerName());

            log.Error("socket error", exception);
            PriorWebsocketSuccess = false;

            //prevent endless loop
            if (_errorCount == 0)
            {
                _errorCount++;
                Emit(EventError, exception);
                OnClose("transport error", exception);
            }
        }

        private enum ReadyStateEnum
        {
            Opening,
            Open,
            Closing,
            Closed
        }

        private class EventDrainListener : IListener
        {
            private readonly Socket _socket;

            public EventDrainListener(Socket socket)
            {
                this._socket = socket;
            }

            void IListener.Call(params object[] args)
            {
                _socket.OnDrain();
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class EventPacketListener : IListener
        {
            private readonly Socket _socket;

            public EventPacketListener(Socket socket)
            {
                this._socket = socket;
            }

            void IListener.Call(params object[] args)
            {
                _socket.OnPacket(args.Length > 0 ? (Packet) args[0] : null);
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class EventErrorListener : IListener
        {
            private readonly Socket _socket;

            public EventErrorListener(Socket socket)
            {
                this._socket = socket;
            }

            public void Call(params object[] args)
            {
                _socket.OnError(args.Length > 0 ? (Exception) args[0] : null);
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class EventCloseListener : IListener
        {
            private readonly Socket _socket;

            public EventCloseListener(Socket socket)
            {
                this._socket = socket;
            }

            public void Call(params object[] args)
            {
                _socket.OnClose("transport close");
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }


        public class Options : Transport.Options
        {
            public string Host;
            public string QueryString;

            public bool RememberUpgrade;

            public ImmutableList<string> Transports;

            public bool Upgrade = true;

            public static Options FromUri(Uri uri, Options opts)
            {
                if (opts == null) opts = new Options();

                opts.Host = uri.Host;
                opts.Secure = uri.Scheme == "https" || uri.Scheme == "wss";
                opts.Port = uri.Port;

                if (!string.IsNullOrEmpty(uri.Query)) opts.QueryString = uri.Query;

                return opts;
            }
        }

        private class OnHeartbeatAsListener : IListener
        {
            private readonly Socket _socket;

            public OnHeartbeatAsListener(Socket socket)
            {
                this._socket = socket;
            }

            void IListener.Call(params object[] args)
            {
                _socket.OnHeartbeat(args.Length > 0 ? (long) args[0] : 0);
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class ProbeParameters
        {
            public ImmutableList<Transport> Transport { get; set; }
            public ImmutableList<bool> Failed { get; set; }
            public ImmutableList<Action> Cleanup { get; set; }
            public Socket Socket { get; set; }
        }

        private class OnTransportOpenListener : IListener
        {
            private readonly ProbeParameters _parameters;


            public OnTransportOpenListener(ProbeParameters parameters)
            {
                _parameters = parameters;
            }

            void IListener.Call(params object[] args)
            {
                if (_parameters.Failed[0]) return;

                var packet = new Packet(Packet.PING, "probe");
                _parameters.Transport[0].Once(Transport.EventPacket, new ProbeEventPacketListener(this));
                _parameters.Transport[0].Send(ImmutableList<Packet>.Empty.Add(packet));
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }

            private class ProbeEventPacketListener : IListener
            {
                private readonly OnTransportOpenListener _onTransportOpenListener;

                public ProbeEventPacketListener(OnTransportOpenListener onTransportOpenListener)
                {
                    _onTransportOpenListener = onTransportOpenListener;
                }


                void IListener.Call(params object[] args)
                {
                    if (_onTransportOpenListener._parameters.Failed[0]) return;
                    var log = LogManager.GetLogger(Global.CallerName());

                    var msg = (Packet) args[0];
                    if (Packet.PONG == msg.Type && "probe" == (string) msg.Data)
                    {
                        //log.Info(
                        //    string.Format("probe transport '{0}' pong",
                        //        _onTransportOpenListener.Parameters.Transport[0].Name));

                        _onTransportOpenListener._parameters.Socket._upgrading = true;
                        _onTransportOpenListener._parameters.Socket.Emit(EventUpgrading,
                            _onTransportOpenListener._parameters.Transport[0]);
                        PriorWebsocketSuccess = WebSocket.NAME ==
                                                _onTransportOpenListener._parameters.Transport[0].Name;

                        //log.Info(
                        //    string.Format("pausing current transport '{0}'",
                        //        _onTransportOpenListener.Parameters.Socket.Transport.Name));
                        ((Polling) _onTransportOpenListener._parameters.Socket.Transport).Pause(
                            () =>
                            {
                                if (_onTransportOpenListener._parameters.Failed[0])
                                {
                                    // reset upgrading flag and resume polling
                                    ((Polling) _onTransportOpenListener._parameters.Socket.Transport).Resume();
                                    _onTransportOpenListener._parameters.Socket._upgrading = false;
                                    _onTransportOpenListener._parameters.Socket.Flush();
                                    return;
                                }

                                if (ReadyStateEnum.Closed == _onTransportOpenListener._parameters.Socket._readyState ||
                                    ReadyStateEnum.Closing == _onTransportOpenListener._parameters.Socket._readyState)
                                    return;

                                log.Info("changing transport and sending upgrade packet");

                                _onTransportOpenListener._parameters.Cleanup[0]();

                                _onTransportOpenListener._parameters.Socket.SetTransport(
                                    _onTransportOpenListener._parameters.Transport[0]);
                                var packetList =
                                    ImmutableList<Packet>.Empty.Add(new Packet(Packet.UPGRADE));
                                try
                                {
                                    _onTransportOpenListener._parameters.Transport[0].Send(packetList);

                                    _onTransportOpenListener._parameters.Socket._upgrading = false;
                                    _onTransportOpenListener._parameters.Socket.Flush();

                                    _onTransportOpenListener._parameters.Socket.Emit(EventUpgrade,
                                        _onTransportOpenListener._parameters.Transport[0]);
                                    _onTransportOpenListener._parameters.Transport =
                                        _onTransportOpenListener._parameters.Transport.RemoveAt(0);
                                }
                                catch (Exception e)
                                {
                                    log.Error("", e);
                                }
                            });
                    }
                    else
                    {
                        log.Info(string.Format("probe transport '{0}' failed",
                            _onTransportOpenListener._parameters.Transport[0].Name));

                        var err = new EngineIOException("probe error");
                        _onTransportOpenListener._parameters.Socket.Emit(EventUpgradeError, err);
                    }
                }


                public int CompareTo(IListener other)
                {
                    return GetId().CompareTo(other.GetId());
                }

                public int GetId()
                {
                    return 0;
                }
            }
        }

        private class FreezeTransportListener : IListener
        {
            private readonly ProbeParameters _parameters;

            public FreezeTransportListener(ProbeParameters parameters)
            {
                _parameters = parameters;
            }

            void IListener.Call(params object[] args)
            {
                if (_parameters.Failed[0]) return;

                _parameters.Failed = _parameters.Failed.SetItem(0, true);

                _parameters.Cleanup[0]();

                if (_parameters.Transport.Count < 1) return;

                _parameters.Transport[0].Close();
                _parameters.Transport = ImmutableList<Transport>.Empty;
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class ProbingOnErrorListener : IListener
        {
            private readonly IListener _freezeTransport;
            private readonly Socket _socket;
            private readonly ImmutableList<Transport> _transport;

            public ProbingOnErrorListener(Socket socket, ImmutableList<Transport> transport, IListener freezeTransport)
            {
                _socket = socket;
                _transport = transport;
                _freezeTransport = freezeTransport;
            }

            void IListener.Call(params object[] args)
            {
                var err = args[0];
                EngineIOException error;
                if (err is Exception)
                    error = new EngineIOException("probe error", (Exception) err);
                else if (err is string)
                    error = new EngineIOException("probe error: " + (string) err);
                else
                    error = new EngineIOException("probe error");
                error.Transport = _transport[0].Name;

                _freezeTransport.Call();

                var log = LogManager.GetLogger(Global.CallerName());

                log.Info(string.Format("probe transport \"{0}\" failed because of error: {1}", error.Transport, err));
                _socket.Emit(EventUpgradeError, error);
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class ProbingOnTransportCloseListener : IListener
        {
            private readonly IListener _onError;

            public ProbingOnTransportCloseListener(ProbingOnErrorListener onError)
            {
                _onError = onError;
            }

            void IListener.Call(params object[] args)
            {
                _onError.Call("transport closed");
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class ProbingOnCloseListener : IListener
        {
            private readonly IListener _onError;

            public ProbingOnCloseListener(ProbingOnErrorListener onError)
            {
                _onError = onError;
            }

            void IListener.Call(params object[] args)
            {
                _onError.Call("socket closed");
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }

        private class ProbingOnUpgradeListener : IListener
        {
            private readonly IListener _freezeTransport;
            private readonly ImmutableList<Transport> _transport;

            public ProbingOnUpgradeListener(FreezeTransportListener freezeTransport, ImmutableList<Transport> transport)
            {
                _freezeTransport = freezeTransport;
                _transport = transport;
            }

            void IListener.Call(params object[] args)
            {
                var to = (Transport) args[0];
                if (_transport[0] != null && to.Name != _transport[0].Name)
                {
                    var log = LogManager.GetLogger(Global.CallerName());

                    log.Info(string.Format("'{0}' works - aborting '{1}'", to.Name, _transport[0].Name));
                    _freezeTransport.Call();
                }
            }

            public int CompareTo(IListener other)
            {
                return GetId().CompareTo(other.GetId());
            }

            public int GetId()
            {
                return 0;
            }
        }
    }
}