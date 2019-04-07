using System.Net;

namespace Xky.Socket.Engine.Modules
{
    public class ServerCertificate
    {
        static ServerCertificate()
        {
            Ignore = false;
        }

        public static bool Ignore { get; set; }

        public static void IgnoreServerCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            Ignore = true;
        }
    }
}