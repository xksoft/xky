using System;

namespace Xky.Socket.Client
{
    public class SocketIOException : Exception
    {
        public object code;
        public string Transport;

        public SocketIOException(string message)
            : base(message)
        {
        }


        public SocketIOException(Exception cause)
            : base("", cause)
        {
        }

        public SocketIOException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}