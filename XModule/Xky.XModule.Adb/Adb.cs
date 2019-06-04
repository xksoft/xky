using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xky.Core;

namespace Xky.XModule.Adb
{
    public class Adb : Core.XModule
    {
       
        public int id = 0;
        public override string Name()
        {
            return "执行Adb命令";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Adb.logo.png");
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
        public override string Description()
        {
            return "执行普通Adb或Shell命令";
        }

        public override void Start()
        {
   
        }
        
        public override bool ShowUserControl()
        {
            
           
            var panel = new ModulePanel();
            panel.device = Devices[0];
            Core.Client.ShowDialogPanel(panel);
            return true;

        }
    }
}
