using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using XSocket.Engine.Client;
using XSocket.Engine.ComponentEmitter;
using XSocket.Engine.Modules;
using XSocket.Engine.Parser;

namespace XSocket.Client.Transports
{
    public class Polling : Transport
    {
        public static readonly string NAME = "polling";
        public static readonly string EVENT_POLL = "poll";
        public static readonly string EVENT_POLL_COMPLETE = "pollComplete";

        private bool IsPolling;

        public Polling(Options opts) : base(opts)
        {
            Name = NAME;
        }


        protected override void DoOpen()
        {
            Poll();
        }

        public void Pause(Action onPause)
        {
            //var log = LogManager.GetLogger(Global.CallerName());

            ReadyState = ReadyStateEnum.Paused;
            Action pause = () =>
            {
                //log.Info("paused");
                ReadyState = ReadyStateEnum.Paused;
                onPause();
            };

            if (IsPolling || !Writable)
            {
                var total = new[] {0};


                if (IsPolling)
                {
                    //log.Info("we are currently polling - waiting to pause");
                    total[0]++;
                    Once(EVENT_POLL_COMPLETE, new PauseEventPollCompleteListener(total, pause));
                }

                if (!Writable)
                {
                    //log.Info("we are currently writing - waiting to pause");
                    total[0]++;
                    Once(EventDrain, new PauseEventDrainListener(total, pause));
                }
            }
            else
            {
                pause();
            }
        }

        public void Resume()
        {
            if (ReadyState == ReadyStateEnum.Paused)
                ReadyState = ReadyStateEnum.Open;
        }


        private void Poll()
        {
            //var log = LogManager.GetLogger(Global.CallerName());

            //log.Info("polling");
            IsPolling = true;
            DoPoll();
            Emit(EVENT_POLL);
        }


        protected override void OnData(string data)
        {
            _onData(data);
        }

        protected override void OnData(byte[] data)
        {
            _onData(data);
        }


        private void _onData(object data)
        {
            var log = LogManager.GetLogger(Global.CallerName());

            log.Info(string.Format("polling got data {0}", data));
            var callback = new DecodePayloadCallback(this);
            if (data is string)
                XSocket.Engine.Parser.Parser.DecodePayload((string) data, callback);
            else if (data is byte[]) XSocket.Engine.Parser.Parser.DecodePayload((byte[]) data, callback);

            if (ReadyState != ReadyStateEnum.Closed)
            {
                IsPolling = false;
                log.Info("ReadyState != ReadyStateEnum.CLOSED");
                Emit(EVENT_POLL_COMPLETE);

                if (ReadyState == ReadyStateEnum.Open)
                    Poll();
                else
                    log.Info(string.Format("ignoring poll - transport state {0}", ReadyState));
            }
        }

        protected override void DoClose()
        {
            var log = LogManager.GetLogger(Global.CallerName());

            var closeListener = new CloseListener(this);

            if (ReadyState == ReadyStateEnum.Open)
            {
                log.Info("transport open - closing");
                closeListener.Call();
            }
            else
            {
                // in case we're trying to close while
                // handshaking is in progress (engine.io-client GH-164)
                log.Info("transport not open - deferring close");
                Once(EventOpen, closeListener);
            }
        }


        protected override void Write(ImmutableList<Packet> packets)
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("Write packets.Count = " + packets.Count);

            Writable = false;

            var callback = new SendEncodeCallback(this);
            XSocket.Engine.Parser.Parser.EncodePayload(packets.ToArray(), callback);
        }

        public string Uri()
        {
            //var query = this.Query;
            var query = new Dictionary<string, string>(Query);
            //if (Query == null)
            //{
            //    query = new Dictionary<string, string>();
            //}
            var schema = Secure ? "https" : "http";
            var portString = "";

            if (TimestampRequests) query.Add(TimestampParam, DateTime.Now.Ticks + "-" + Timestamps++);

            query.Add("b64", "1");


            var _query = ParseQS.Encode(query);

            if (Port > 0 && ("https" == schema && Port != 443
                             || "http" == schema && Port != 80))
                portString = ":" + Port;

            if (_query.Length > 0) _query = "?" + _query;

            return schema + "://" + Hostname + portString + Path + _query;
        }

        protected virtual void DoWrite(byte[] data, Action action)
        {
        }

        protected virtual void DoPoll()
        {
        }

        private class PauseEventDrainListener : IListener
        {
            private readonly Action pause;
            private readonly int[] total;

            public PauseEventDrainListener(int[] total, Action pause)
            {
                this.total = total;
                this.pause = pause;
            }

            public void Call(params object[] args)
            {
                //var log = LogManager.GetLogger(Global.CallerName());

                //log.Info("pre-pause writing complete");
                if (--total[0] == 0) pause();
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

        private class PauseEventPollCompleteListener : IListener
        {
            private readonly Action pause;
            private readonly int[] total;

            public PauseEventPollCompleteListener(int[] total, Action pause)
            {
                this.total = total;
                this.pause = pause;
            }

            public void Call(params object[] args)
            {
                //var log = LogManager.GetLogger(Global.CallerName());

                //log.Info("pre-pause polling complete");
                if (--total[0] == 0) pause();
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


        private class DecodePayloadCallback : IDecodePayloadCallback
        {
            private readonly Polling polling;

            public DecodePayloadCallback(Polling polling)
            {
                this.polling = polling;
            }

            public bool Call(Packet packet, int index, int total)
            {
                if (polling.ReadyState == ReadyStateEnum.Opening) polling.OnOpen();

                if (packet.Type == Packet.CLOSE)
                {
                    polling.OnClose();
                    return false;
                }

                polling.OnPacket(packet);
                return true;
            }
        }

        private class CloseListener : IListener
        {
            private readonly Polling polling;

            public CloseListener(Polling polling)
            {
                this.polling = polling;
            }

            public void Call(params object[] args)
            {
                //var log = LogManager.GetLogger(Global.CallerName());

                //log.Info("writing close packet");
                var packets = ImmutableList<Packet>.Empty;
                packets = packets.Add(new Packet(Packet.CLOSE));
                polling.Write(packets);
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


        public class SendEncodeCallback : IEncodeCallback
        {
            private readonly Polling polling;

            public SendEncodeCallback(Polling polling)
            {
                this.polling = polling;
            }

            public void Call(object data)
            {
                //var log = LogManager.GetLogger(Global.CallerName());
                //log.Info("SendEncodeCallback data = " + data);

                var byteData = (byte[]) data;
                polling.DoWrite(byteData, () =>
                {
                    polling.Writable = true;
                    polling.Emit(EventDrain);
                });
            }
        }
    }
}