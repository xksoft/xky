using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.Adb
{
    public class Adb : Core.XModule
    {
        public override string Name()
        {
            return "执行Adb命令";
        }
        public override bool IsBackground()
        {
            return false;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Adb.logo.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[前台模块]执行普通Adb或Shell命令";
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
