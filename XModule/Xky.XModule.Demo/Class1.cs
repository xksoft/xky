using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xky.Core.Model;

namespace Xky.XModule.Demo
{
    public class MyDemo : Core.XModule
    {
        public override string Description()
        {
            return "模块描述";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.logo.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public override bool IsBackground()
        {
            return true;
        }

        public override string Name()
        {
            return "模块名称";
        }

        public override void Start()
        {
           Response res= Device.ScriptEngine.FindAllUiObject();
        }
    }
}
