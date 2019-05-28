using System;
using System.IO;
using System.Reflection;

namespace Xky.XModule.Test
{
    public class Test : Core.XModule
    {
        public int t = 0;
        public override string Description()
        {
            return "描述";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public override bool IsBackground()
        {
            return false;
        }

        public override string Name()
        {
            return "模块名称";
        }
       
        public override bool ShowUserControl()
        {
            Init();
            return true;
        }
        public override void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                Device.ScriptEngine.Toast(i+"-"+t.ToString());
            }
            
        }
    }
}
