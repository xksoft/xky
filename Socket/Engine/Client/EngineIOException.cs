using System;

namespace XSocket.Engine.Client
{
    public class EngineIOException : Exception
    {
        public object code;
        public string Transport;

        public EngineIOException(string message)
            : base(message)
        {
        }


        public EngineIOException(Exception cause)
            : base("", cause)
        {
        }

        public EngineIOException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}