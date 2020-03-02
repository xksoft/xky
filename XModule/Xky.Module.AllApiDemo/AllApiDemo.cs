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
    public class AllApiDemo: XCore.XModule
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
            if (myStream != null)
            {
                byte[] bytes = new byte[myStream.Length];
                myStream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
            else
            {
                Console.WriteLine("模块图标路径有误，无法加载！");
                return null;
            }

        }
        public override bool SupportBatchControl()
        {
            return false;
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
            panel.device = Devices[0];
            XCore.Client.ShowDialogPanel(panel);
            return true;

        }
    }
}
