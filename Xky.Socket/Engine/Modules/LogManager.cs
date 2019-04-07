using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Xky.Socket.Engine.Modules
{
    public class LogManager
    {
        private const string LogFilePath = "XunitTrace.log";

        private static readonly LogManager EmptyLogger = new LogManager(null);

        private static StreamWriter writer;

        private readonly string type;

        public LogManager(string type)
        {
            this.type = type;
        }

        public static bool Enabled { get; set; }

        private static StreamWriter Writer
        {
            get
            {
                if (writer == null)
                {
                    var fs = new FileStream(
                        LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    writer = new StreamWriter(fs, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                }

                return writer;
            }
        }

        [Conditional("DEBUG")]
        public void Info(string msg)
        {
            if (!Enabled) return;

            Writer.WriteLine(
                "{0:yyyy-MM-dd HH:mm:ss fff} [] {1} {2}",
                DateTime.Now,
                type,
                Global.StripInvalidUnicodeCharacters(msg));
        }

        [Conditional("DEBUG")]
        public void Error(string p, Exception exception)
        {
            Info($"ERROR {p} {exception.Message} {exception.StackTrace}");
            if (exception.InnerException != null)
                Info(
                    $"ERROR exception.InnerException {p} {exception.InnerException.Message} {exception.InnerException.StackTrace}");
        }


        [Conditional("DEBUG")]
        internal void Error(Exception e)
        {
            Error("", e);
        }

        #region Statics

        public static void SetupLogManager()
        {
        }

        public static LogManager GetLogger(string type)
        {
            return new LogManager(type);
        }

        public static LogManager GetLogger(Type type)
        {
            return GetLogger(type.ToString());
        }

        public static LogManager GetLogger(MethodBase methodBase)
        {
#if DEBUG
            var declaringType = methodBase.DeclaringType != null
                ? methodBase.DeclaringType.ToString()
                : string.Empty;
            var fullType = string.Format("{0}#{1}", declaringType, methodBase.Name);
            return GetLogger(fullType);
#else
            return EmptyLogger;
#endif
        }

        #endregion
    }
}