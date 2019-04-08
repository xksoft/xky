using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xky.Socket.Engine.ComponentEmitter;
using Xky.Socket.Engine.Modules;

namespace Xky.Socket.Engine.Client.Transports
{
    public class PollingXHR : Polling
    {
        private XHRRequest sendXhr;

        public PollingXHR(Options options) : base(options)
        {
        }

        protected XHRRequest Request()
        {
            return Request(null);
        }


        protected XHRRequest Request(XHRRequest.RequestOptions opts)
        {
            if (opts == null) opts = new XHRRequest.RequestOptions();
            opts.Uri = Uri();

            opts.ExtraHeaders = ExtraHeaders;

            var req = new XHRRequest(opts);


            req.On(EventRequestHeaders, new EventRequestHeadersListener(this))
                .On(EventResponseHeaders, new EventResponseHeadersListener(this));

            return req;
        }


        protected override void DoWrite(byte[] data, Action action)
        {
            var opts = new XHRRequest.RequestOptions {Method = "POST", Data = data, CookieHeaderValue = Cookie};
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("DoWrite data = " + data);
            //try
            //{
            //    var dataString = BitConverter.ToString(data);
            //    log.Info(string.Format("DoWrite data {0}", dataString));
            //}
            //catch (Exception e)
            //{
            //    log.Error(e);
            //}

            sendXhr = Request(opts);
            sendXhr.On(EventSuccess, new SendEventSuccessListener(action));
            sendXhr.On(EventError, new SendEventErrorListener(this));
            sendXhr.Create();
        }


        protected override void DoPoll()
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("xhr DoPoll");
            var opts = new XHRRequest.RequestOptions {CookieHeaderValue = Cookie};
            sendXhr = Request(opts);
            sendXhr.On(EventData, new DoPollEventDataListener(this));
            sendXhr.On(EventError, new DoPollEventErrorListener(this));

            sendXhr.Create();
        }

        private class EventRequestHeadersListener : IListener
        {
            private readonly PollingXHR pollingXHR;

            public EventRequestHeadersListener(PollingXHR pollingXHR)
            {
                this.pollingXHR = pollingXHR;
            }

            public void Call(params object[] args)
            {
                // Never execute asynchronously for support to modify headers.
                pollingXHR.Emit(EventRequestHeaders, args[0]);
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

        private class EventResponseHeadersListener : IListener
        {
            private readonly PollingXHR pollingXHR;

            public EventResponseHeadersListener(PollingXHR pollingXHR)
            {
                this.pollingXHR = pollingXHR;
            }

            public void Call(params object[] args)
            {
                pollingXHR.Emit(EventResponseHeaders, args[0]);
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

        private class SendEventErrorListener : IListener
        {
            private readonly PollingXHR pollingXHR;

            public SendEventErrorListener(PollingXHR pollingXHR)
            {
                this.pollingXHR = pollingXHR;
            }

            public void Call(params object[] args)
            {
                var err = args.Length > 0 && args[0] is Exception ? (Exception) args[0] : null;
                pollingXHR.OnError("xhr post error", err);
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

        private class SendEventSuccessListener : IListener
        {
            private readonly Action action;

            public SendEventSuccessListener(Action action)
            {
                this.action = action;
            }

            public void Call(params object[] args)
            {
                action();
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

        private class DoPollEventDataListener : IListener
        {
            private readonly PollingXHR pollingXHR;

            public DoPollEventDataListener(PollingXHR pollingXHR)
            {
                this.pollingXHR = pollingXHR;
            }


            public void Call(params object[] args)
            {
                var arg = args.Length > 0 ? args[0] : null;
                if (arg is string)
                    pollingXHR.OnData((string) arg);
                else if (arg is byte[]) pollingXHR.OnData((byte[]) arg);
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

        private class DoPollEventErrorListener : IListener
        {
            private readonly PollingXHR pollingXHR;

            public DoPollEventErrorListener(PollingXHR pollingXHR)
            {
                this.pollingXHR = pollingXHR;
            }

            public void Call(params object[] args)
            {
                var err = args.Length > 0 && args[0] is Exception ? (Exception) args[0] : null;
                pollingXHR.OnError("xhr poll error", err);
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


        public class XHRRequest : Emitter
        {
            private readonly string CookieHeaderValue;
            private readonly byte[] Data;
            private readonly Dictionary<string, string> ExtraHeaders;
            private readonly string Method;
            private readonly string Uri;
            private HttpWebRequest Xhr;

            public XHRRequest(RequestOptions options)
            {
                Method = options.Method ?? "GET";
                Uri = options.Uri;
                Data = options.Data;
                CookieHeaderValue = options.CookieHeaderValue;
                ExtraHeaders = options.ExtraHeaders;
            }

            public void Create()
            {
                var log = LogManager.GetLogger(Global.CallerName());

                try
                {
                    log.Info(string.Format("xhr open {0}: {1}", Method, Uri));
                    Xhr = (HttpWebRequest) WebRequest.Create(Uri);
                    Xhr.Method = Method;
                    if (CookieHeaderValue != null) Xhr.Headers.Add("Cookie", CookieHeaderValue);
                    if (ExtraHeaders != null)
                        foreach (var header in ExtraHeaders)
                            Xhr.Headers.Add(header.Key, header.Value);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    OnError(e);
                    return;
                }


                if (Method == "POST") Xhr.ContentType = "application/octet-stream";

                try
                {
                    if (Data != null)
                    {
                        Xhr.ContentLength = Data.Length;

                        using (var requestStream = Xhr.GetRequestStream())
                        {
                            requestStream.WriteAsync(Data, 0, Data.Length).Wait();
                        }
                    }

                    Task.Run(() =>
                    {
                        var log2 = LogManager.GetLogger(Global.CallerName());
                        log2.Info("Task.Run Create start");
                        using (var res = Xhr.GetResponse())
                        {
                            log.Info("Xhr.GetResponse ");

                            var responseHeaders = new Dictionary<string, string>();
                            for (var i = 0; i < res.Headers.Count; i++)
                                responseHeaders.Add(res.Headers.Keys[i], res.Headers[i]);
                            OnResponseHeaders(responseHeaders);

                            var contentType = res.Headers["Content-Type"];


                            using (var resStream = res.GetResponseStream())
                            {
                                Debug.Assert(resStream != null, "resStream != null");
                                if (contentType.Equals("application/octet-stream",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    var buffer = new byte[16 * 1024];
                                    using (var ms = new MemoryStream())
                                    {
                                        int read;
                                        while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                                            ms.Write(buffer, 0, read);
                                        var a = ms.ToArray();
                                        OnData(a);
                                    }
                                }
                                else
                                {
                                    using (var sr = new StreamReader(resStream))
                                    {
                                        OnData(sr.ReadToEnd());
                                    }
                                }
                            }
                        }

                        log2.Info("Task.Run Create finish");
                    }).Wait();
                }
                catch (IOException e)
                {
                    log.Error("Create call failed", e);
                    OnError(e);
                }
                catch (WebException e)
                {
                    log.Error("Create call failed", e);
                    OnError(e);
                }
                catch (Exception e)
                {
                    log.Error("Create call failed", e);
                    OnError(e);
                }
            }


            private void OnSuccess()
            {
                Emit(EventSuccess);
            }

            private void OnData(string data)
            {
                var log = LogManager.GetLogger(Global.CallerName());
                log.Info("OnData string = " + data);
                Emit(EventData, data);
                OnSuccess();
            }

            private void OnData(byte[] data)
            {
                var log = LogManager.GetLogger(Global.CallerName());
                log.Info("OnData byte[] =" + Encoding.UTF8.GetString(data));
                Emit(EventData, data);
                OnSuccess();
            }

            private void OnError(Exception err)
            {
                Emit(EventError, err);
            }

            private void OnRequestHeaders(Dictionary<string, string> headers)
            {
                Emit(EventRequestHeaders, headers);
            }

            private void OnResponseHeaders(Dictionary<string, string> headers)
            {
                Emit(EventResponseHeaders, headers);
            }

            public class RequestOptions
            {
                public string CookieHeaderValue;
                public byte[] Data;
                public Dictionary<string, string> ExtraHeaders;
                public string Method;
                public string Uri;
            }
        }
    }
}