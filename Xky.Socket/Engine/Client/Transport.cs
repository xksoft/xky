using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Authentication;
using System.Text;
using Xky.Socket.Engine.ComponentEmitter;
using Xky.Socket.Engine.Modules;
using Xky.Socket.Engine.Parser;

namespace Xky.Socket.Engine.Client
{
    public abstract class Transport : Emitter
    {
        public static readonly string EVENT_OPEN = "open";
        public static readonly string EVENT_CLOSE = "close";
        public static readonly string EVENT_PACKET = "packet";
        public static readonly string EVENT_DRAIN = "drain";
        public static readonly string EVENT_ERROR = "error";
        public static readonly string EVENT_SUCCESS = "success";
        public static readonly string EVENT_DATA = "data";
        public static readonly string EVENT_REQUEST_HEADERS = "requestHeaders";
        public static readonly string EVENT_RESPONSE_HEADERS = "responseHeaders";

        protected static int Timestamps = 0;

        private bool _writeable;
        protected bool Agent;
        protected string Cookie;

        protected Dictionary<string, string> ExtraHeaders;
        protected bool ForceBase64;
        protected bool ForceJsonp;
        protected string Hostname;

        public string Name;
        protected string Path;
        protected int Port;
        public Dictionary<string, string> Query;


        protected ReadyStateEnum ReadyState = ReadyStateEnum.CLOSED;

        protected bool Secure;
        protected Socket Socket;
        protected SslProtocols SslProtocols;
        protected string TimestampParam;
        protected bool TimestampRequests;

        protected Transport(Options options)
        {
            Path = options.Path;
            Hostname = options.Hostname;
            Port = options.Port;
            Secure = options.Secure;
            SslProtocols = options.SslProtocols;
            Query = options.Query;
            TimestampParam = options.TimestampParam;
            TimestampRequests = options.TimestampRequests;
            Socket = options.Socket;
            Agent = options.Agent;
            ForceBase64 = options.ForceBase64;
            ForceJsonp = options.ForceJsonp;
            Cookie = options.GetCookiesAsString();
            ExtraHeaders = options.ExtraHeaders;
        }

        public bool Writable
        {
            get => _writeable;
            set
            {
                var log = LogManager.GetLogger(Global.CallerName());
                log.Info(string.Format("Writable: {0} sid={1}", value, Socket.Id));
                _writeable = value;
            }
        }

        public int MyProperty { get; set; }

        protected Transport OnError(string message, Exception exception)
        {
            Exception err = new EngineIOException(message, exception);
            Emit(EVENT_ERROR, err);
            return this;
        }

        protected void OnOpen()
        {
            ReadyState = ReadyStateEnum.OPEN;
            Writable = true;
            Emit(EVENT_OPEN);
        }

        protected void OnClose()
        {
            ReadyState = ReadyStateEnum.CLOSED;
            Emit(EVENT_CLOSE);
        }


        protected virtual void OnData(string data)
        {
            OnPacket(Parser.Parser.DecodePacket(data));
        }

        protected virtual void OnData(byte[] data)
        {
            OnPacket(Parser.Parser.DecodePacket(data));
        }

        protected void OnPacket(Packet packet)
        {
            Emit(EVENT_PACKET, packet);
        }


        public Transport Open()
        {
            if (ReadyState == ReadyStateEnum.CLOSED)
            {
                ReadyState = ReadyStateEnum.OPENING;
                DoOpen();
            }

            return this;
        }

        public Transport Close()
        {
            if (ReadyState == ReadyStateEnum.OPENING || ReadyState == ReadyStateEnum.OPEN)
            {
                DoClose();
                OnClose();
            }

            return this;
        }

        public Transport Send(ImmutableList<Packet> packets)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("Send called with packets.Count: " + packets.Count);
            var count = packets.Count;
            if (ReadyState == ReadyStateEnum.OPEN)
                Write(packets);
            else
                throw new EngineIOException("Transport not open");
            return this;
        }


        protected abstract void DoOpen();

        protected abstract void DoClose();

        protected abstract void Write(ImmutableList<Packet> packets);

        protected enum ReadyStateEnum
        {
            OPENING,
            OPEN,
            CLOSED,
            PAUSED
        }


        public class Options
        {
            public bool Agent = false;
            public Dictionary<string, string> Cookies = new Dictionary<string, string>();
            public Dictionary<string, string> ExtraHeaders = new Dictionary<string, string>();
            public bool ForceBase64 = false;
            public bool ForceJsonp = false;
            public string Hostname;
            public bool IgnoreServerCertificateValidation = false;
            public string Path;
            public int PolicyPort;
            public int Port;
            public Dictionary<string, string> Query;
            public bool Secure = false;
            internal Socket Socket;
            public SslProtocols SslProtocols;
            public string TimestampParam;
            public bool TimestampRequests = true;

            public string GetCookiesAsString()
            {
                var result = new StringBuilder();
                var first = true;
                foreach (var item in Cookies)
                {
                    if (!first) result.Append("; ");
                    result.Append(string.Format("{0}={1}", item.Key, item.Value));
                    first = false;
                }

                return result.ToString();
            }
        }
    }
}