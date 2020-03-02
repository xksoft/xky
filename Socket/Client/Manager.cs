using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xky.Socket.Engine.ComponentEmitter;
using Xky.Socket.Engine.Modules;
using Xky.Socket.Engine.Thread;
using Xky.Socket.Parser;

namespace Xky.Socket.Client
{
    public class Manager : Emitter
    {
        public enum ReadyStateEnum
        {
            OPENING,
            OPEN,
            CLOSED
        }

        public static readonly string EVENT_ENGINE = "engine";
        public static readonly string EVENT_OPEN = "open";
        public static readonly string EVENT_CLOSE = "close";
        public static readonly string EVENT_PACKET = "packet";
        public static readonly string EVENT_ERROR = "error";
        public static readonly string EVENT_CONNECT_ERROR = "connect_error";
        public static readonly string EVENT_CONNECT_TIMEOUT = "connect_timeout";
        public static readonly string EVENT_RECONNECT = "reconnect";
        public static readonly string EVENT_RECONNECT_ERROR = "reconnect_error";
        public static readonly string EVENT_RECONNECT_FAILED = "reconnect_failed";
        public static readonly string EVENT_RECONNECT_ATTEMPT = "reconnect_attempt";
        public static readonly string EVENT_RECONNECTING = "reconnecting";
        private readonly bool AutoConnect;
        private readonly Parser.Parser.Decoder Decoder;
        private readonly Parser.Parser.Encoder Encoder;
        private readonly HashSet<Socket> OpeningSockets;
        private readonly Xky.Socket.Engine.Client.Socket.Options Opts;
        private readonly List<Packet> PacketBuffer;
        private readonly ConcurrentQueue<On.IHandle> Subs;
        private readonly Uri Uri;

        private bool _reconnection;
        private int _reconnectionAttempts;
        private long _reconnectionDelay;
        private long _reconnectionDelayMax;
        private long _timeout;
        private int Attempts;

        private bool Encoding;
        /*package*/

        public Xky.Socket.Engine.Client.Socket EngineSocket;

        /**
         * This ImmutableDictionary can be accessed from outside of EventThread.
         */
        private ImmutableDictionary<string, Socket> Nsps;
        private bool OpenReconnect;


        /*package*/
        public ReadyStateEnum ReadyState = ReadyStateEnum.CLOSED;
        private bool Reconnecting;
        private bool SkipReconnect;

        public Manager() : this(null, null)
        {
        }

        public Manager(Uri uri) : this(uri, null)
        {
        }

        public Manager(Options opts) : this(null, opts)
        {
        }

        public Manager(Uri uri, Options opts)
        {
            if (opts == null) opts = new Options();
            if (opts.Path == null) opts.Path = "/socket.io";
            Opts = opts;
            Nsps = ImmutableDictionary.Create<string, Socket>();
            Subs = new ConcurrentQueue<On.IHandle>();
            Reconnection(opts.Reconnection);
            ReconnectionAttempts(opts.ReconnectionAttempts != 0 ? opts.ReconnectionAttempts : int.MaxValue);
            ReconnectionDelay(opts.ReconnectionDelay != 0 ? opts.ReconnectionDelay : 1000);
            ReconnectionDelayMax(opts.ReconnectionDelayMax != 0 ? opts.ReconnectionDelayMax : 5000);
            Timeout(opts.Timeout < 0 ? 20000 : opts.Timeout);
            ReadyState = ReadyStateEnum.CLOSED;
            Uri = uri;
            Attempts = 0;
            Encoding = false;
            PacketBuffer = new List<Packet>();
            OpeningSockets = new HashSet<Socket>();
            Encoder = new Parser.Parser.Encoder();
            Decoder = new Parser.Parser.Decoder();
            AutoConnect = opts.AutoConnect;
            if (AutoConnect) Open();
        }

        private void EmitAll(string eventString, params object[] args)
        {
            Emit(eventString, args);
            foreach (var socket in Nsps.Values) socket.Emit(eventString, args);
        }

        public bool Reconnection()
        {
            return _reconnection;
        }

        private Manager Reconnection(bool v)
        {
            _reconnection = v;
            return this;
        }


        public int ReconnectionAttempts()
        {
            return _reconnectionAttempts;
        }

        private Manager ReconnectionAttempts(int v)
        {
            _reconnectionAttempts = v;
            return this;
        }

        public long ReconnectionDelay()
        {
            return _reconnectionDelay;
        }

        private Manager ReconnectionDelay(long v)
        {
            _reconnectionDelay = v;
            return this;
        }

        public long ReconnectionDelayMax()
        {
            return _reconnectionDelayMax;
        }

        private Manager ReconnectionDelayMax(long v)
        {
            _reconnectionDelayMax = v;
            return this;
        }

        public long Timeout()
        {
            return _timeout;
        }

        private Manager Timeout(long v)
        {
            _timeout = v;
            return this;
        }

        private void MaybeReconnectOnOpen()
        {
            if (!OpenReconnect && !Reconnecting && _reconnection)
            {
                OpenReconnect = true;
                Reconnect();
            }
        }

        public Manager Open()
        {
            return Open(null);
        }

        private Manager Open(IOpenCallback fn)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info(string.Format("readyState {0}", ReadyState));
            if (ReadyState == ReadyStateEnum.OPEN) return this;

            log.Info(string.Format("opening {0}", Uri));
            EngineSocket = new Engine(Uri, Opts);
            var socket = EngineSocket;
            Emit(EVENT_ENGINE, socket);

            ReadyState = ReadyStateEnum.OPENING;
            OpeningSockets.Add(Socket(Uri.AbsolutePath));
            SkipReconnect = false;

            var openSub = Client.On.Create(socket, Xky.Socket.Engine.Client.Socket.EventOpen, new ListenerImpl(() =>
            {
                OnOpen();
                if (fn != null) fn.Call(null);
            }));

            var errorSub = Client.On.Create(socket, Xky.Socket.Engine.Client.Socket.EventError, new ListenerImpl(
                data =>
                {
                    log.Info("connect_error");
                    Cleanup();
                    ReadyState = ReadyStateEnum.CLOSED;
                    EmitAll(EVENT_CONNECT_ERROR, data);

                    if (fn != null)
                    {
                        var err = new Exception("Connection error");
                        fn.Call(err);
                    }

                    MaybeReconnectOnOpen();
                }));

            if (_timeout >= 0)
            {
                var timeout = (int) _timeout;
                log.Info(string.Format("connection attempt will timeout after {0}", timeout));
                var timer = EasyTimer.SetTimeout(() =>
                {
                    var log2 = LogManager.GetLogger(Global.CallerName());
                    log2.Info("Manager Open start");

                    log2.Info(string.Format("connect attempt timed out after {0}", timeout));
                    openSub.Destroy();
                    socket.Close();
                    socket.Emit(Xky.Socket.Engine.Client.Socket.EventError, new Exception("TimeOout"));
                    EmitAll(EVENT_CONNECT_TIMEOUT, timeout);
                    log2.Info("Manager Open finish");
                }, timeout);

                Subs.Enqueue(new On.ActionHandleImpl(timer.Stop));
                ;
            }

            Subs.Enqueue(openSub);
            Subs.Enqueue(errorSub);
            EngineSocket.Open();

            return this;
        }

        private void OnOpen()
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("open");

            Cleanup();

            ReadyState = ReadyStateEnum.OPEN;
            Emit(EVENT_OPEN);

            var socket = EngineSocket;

            var sub = Client.On.Create(socket, Xky.Socket.Engine.Client.Socket.EventData, new ListenerImpl(data =>
            {
                if (data is string)
                    OnData((string) data);
                else if (data is byte[]) Ondata((byte[]) data);
            }));
            Subs.Enqueue(sub);

            sub = Client.On.Create(Decoder, Parser.Parser.Decoder.EVENT_DECODED,
                new ListenerImpl(data => { OnDecoded((Packet) data); }));
            Subs.Enqueue(sub);

            sub = Client.On.Create(socket, Xky.Socket.Engine.Client.Socket.EventError,
                new ListenerImpl(data => { OnError((Exception) data); }));
            Subs.Enqueue(sub);

            sub = Client.On.Create(socket, Xky.Socket.Engine.Client.Socket.EventClose,
                new ListenerImpl(data => { OnClose((string) data); }));
            Subs.Enqueue(sub);
        }

        private void OnData(string data)
        {
            Decoder.Add(data);
        }

        private void Ondata(byte[] data)
        {
            Decoder.Add(data);
        }

        private void OnDecoded(Packet packet)
        {
            Emit(EVENT_PACKET, packet);
        }

        private void OnError(Exception err)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Error("error", err);
            EmitAll(EVENT_ERROR, err);
        }

        public Socket Socket(string nsp)
        {
            if (Nsps.ContainsKey(nsp)) return Nsps[nsp];

            var socket = new Socket(this, nsp);
            Nsps = Nsps.Add(nsp, socket);

            return socket;
        }

        internal void Destroy(Socket socket)
        {
            OpeningSockets.Remove(socket);
            if (OpeningSockets.Count == 0) Close();
        }


        internal void Packet(Packet packet)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info(string.Format("writing packet {0}", packet));

            if (!Encoding)
            {
                Encoding = true;
                Encoder.Encode(packet, new Parser.Parser.Encoder.CallbackImp(data =>
                {
                    var encodedPackets = data;
                    foreach (var packet1 in encodedPackets)
                        if (packet1 is string)
                            EngineSocket.Write((string) packet1);
                        else if (packet1 is byte[]) EngineSocket.Write((byte[]) packet1);
                    Encoding = false;
                    ProcessPacketQueue();
                }));
            }
            else
            {
                PacketBuffer.Add(packet);
            }
        }

        private void ProcessPacketQueue()
        {
            if (PacketBuffer.Count > 0 && !Encoding)
            {
                var pack = PacketBuffer[0];
                PacketBuffer.Remove(pack);
                Packet(pack);
            }
        }

        private void Cleanup()
        {
            // dequeue and destroy until empty
            while (Subs.TryDequeue(out var sub)) sub.Destroy();
        }

        public void Close()
        {
            SkipReconnect = true;
            Reconnecting = false;

            if (ReadyState != ReadyStateEnum.OPEN) Cleanup();

            ReadyState = ReadyStateEnum.CLOSED;

            if (EngineSocket != null) EngineSocket.Close();
        }


        private void OnClose(string reason)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("start");
            Cleanup();
            ReadyState = ReadyStateEnum.CLOSED;
            Emit(EVENT_CLOSE, reason);
            if (_reconnection && !SkipReconnect) Reconnect();
        }


        private void Reconnect()
        {
            var log = LogManager.GetLogger(Global.CallerName());

            if (Reconnecting || SkipReconnect) return;

            Attempts++;

            if (Attempts > _reconnectionAttempts)
            {
                log.Info("reconnect failed");
                EmitAll(EVENT_RECONNECT_FAILED);
                Reconnecting = false;
            }
            else
            {
                var delay = Attempts * ReconnectionDelay();
                delay = Math.Min(delay, ReconnectionDelayMax());
                log.Info(string.Format("will wait {0}ms before reconnect attempt", delay));

                Reconnecting = true;
                var timer = EasyTimer.SetTimeout(() =>
                {
                    var log2 = LogManager.GetLogger(Global.CallerName());
                    log2.Info("EasyTimer Reconnect start");
                    log2.Info("attempting reconnect");
                    EmitAll(EVENT_RECONNECT_ATTEMPT, Attempts);
                    EmitAll(EVENT_RECONNECTING, Attempts);
                    Open(new OpenCallbackImp(err =>
                    {
                        if (err != null)
                        {
                            log.Error("reconnect attempt error", (Exception) err);
                            Reconnecting = false;
                            Reconnect();
                            EmitAll(EVENT_RECONNECT_ERROR, (Exception) err);
                        }
                        else
                        {
                            log.Info("reconnect success");
                            OnReconnect();
                        }
                    }));
                    log2.Info("EasyTimer Reconnect finish");
                }, (int) delay);

                Subs.Enqueue(new On.ActionHandleImpl(timer.Stop));
            }
        }


        private void OnReconnect()
        {
            var attempts = Attempts;
            Attempts = 0;
            Reconnecting = false;
            EmitAll(EVENT_RECONNECT, attempts);
        }


        public interface IOpenCallback
        {
            void Call(Exception err);
        }

        public class OpenCallbackImp : IOpenCallback
        {
            private readonly Action<object> Fn;

            public OpenCallbackImp(Action<object> fn)
            {
                Fn = fn;
            }

            public void Call(Exception err)
            {
                Fn(err);
            }
        }
    }


    public class Engine : Xky.Socket.Engine.Client.Socket
    {
        public Engine(Uri uri, Options opts) : base(uri, opts)
        {
        }
    }


    public class Options : Xky.Socket.Engine.Client.Socket.Options
    {
        public bool AutoConnect = true;

        public bool Reconnection = true;
        public int ReconnectionAttempts;
        public long ReconnectionDelay;
        public long ReconnectionDelayMax;
        public long Timeout = -1;
    }
}