using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Xky.XModule.Demo
{
    public class Demo : Core.XModule
    {
        public int t = 0;
        public override string Description()
        {
            return "描述";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon.png");
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
            ModulePanel mp = new ModulePanel();
            mp.xmodules = GetXModules();
            Core.Client.ShowDialogPanel(mp);
            return true;
        }
        public override void Start()
        {
            for (int i = 0; i < 20; i++)
            {
                Device.ScriptEngine.Toast(t.ToString() + "-" + i);
                Thread.Sleep(1000);
            }

        }
    }
}
