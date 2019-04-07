using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace Xky.Socket.Client
{
    public class HandshakeData
    {
        public long PingInterval;
        public long PingTimeout;
        public string Sid;
        public ImmutableList<string> Upgrades = ImmutableList<string>.Empty;

        public HandshakeData(string data)
            : this(JObject.Parse(data))
        {
        }

        public HandshakeData(JObject data)
        {
            var upgrades = data.GetValue("upgrades");

            foreach (var e in upgrades) Upgrades = Upgrades.Add(e.ToString());

            Sid = data.GetValue("sid").Value<string>();
            PingInterval = data.GetValue("pingInterval").Value<long>();
            PingTimeout = data.GetValue("pingTimeout").Value<long>();
        }
    }
}