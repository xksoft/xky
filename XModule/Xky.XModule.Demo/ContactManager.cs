using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Xky.XModule.ContactManager
{
    public class ContactManager : Core.XModule
    {
      
        public override string Description()
        {
            return "[支持群控]通讯录管理";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.ContactManager.icon.png");
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

        public override bool IsBackground()
        {
            return false;
        }

        public override string Name()
        {
            return "通讯录管理";
        }

        public override bool ShowUserControl()
        {
            ModulePanel panel = new ModulePanel();
            List<Core.XModule> modulelist = GetXModules();
            panel.xmodules = modulelist;
            Core.Client.ShowDialogPanel(panel);
            return true;
        }
        public override void Start()
        {
          

        }
    }
}
