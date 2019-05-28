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
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "执行普通Adb或Shell命令";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast();
            

            
        }
        
       


        public override bool ShowUserControl()
        {
            
            foreach (var device in base.Devices)
            {
                var xmodule = (Adb)base.Clone();
                xmodule.Device = device;
               
                list.Add(xmodule);
            }
            var panel = new ModulePanel();
            panel.xmodules = list;
            Core.Client.ShowDialogPanel(panel);
            return true;

        }
    }
}
