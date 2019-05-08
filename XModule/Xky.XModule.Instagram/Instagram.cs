using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xky.XModule.Instagram
{
    public class Instagram_打开APP : Core.XModule
    {
        public override string Name()
        {
            return "打开Instagram";
        }
        public override bool IsBackground()
        {
            return false;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Instagram.logo.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[前台模块]打开Instagram";
        }

        public override void Start()
        {
           
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }



        public override bool ShowUserControl()
        {
            return true;

        }
    }
}

