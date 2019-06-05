 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.AppBackup
{
    public class AppBackup_Create : Core.XModule
    {
        public override string Description()
        {
            return "[群控][root]新建一个空的APP备份";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.AppBackup.icon.png");
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
            return true;
        }

        public override string Name()
        {
            return "创建APP备份";
        }
        public override bool ShowUserControl()
        {
            ModulePanel_Create modulePanel_Create = new ModulePanel_Create();
            modulePanel_Create.device = Devices[0];
            Core.Client.ShowDialogPanel(modulePanel_Create);
            return true;

        }
        public override void Start()
        {
           
        }
    }
    public class AppBackup_Manager : Core.XModule
    {
        public override string Description()
        {
            return "[群控][root]管理设备上的APP备份";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.AppBackup.icon.png");
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
            return true;
        }

        public override string Name()
        {
            return "APP备份管理";
        }
       
        public override void Start()
        {

        }
    }
}
