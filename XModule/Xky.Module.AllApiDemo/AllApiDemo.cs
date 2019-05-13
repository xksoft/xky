using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xky.XModule.AllApiDemo;

namespace Xky.XModule.AllApiDemo 
{
    public class AllApiDemo: Core.XModule
    {
        public override string Name()
        {
            return "模块API示例";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.AllApiDemo.logo.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "所有模块支持的API调用演示";
        }

        public override void Start()
        {




        }



        public override bool ShowUserControl()
        {
            var panel = new ModulePanel();
            panel.device = Device;
            Core.Client.ShowDialogPanel(panel);
            return true;

        }
    }
}
