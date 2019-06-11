 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.XModule.AppBackup
{
    public class AppBackup_Create : Core.XModule
    {
        public string PackageName = "";
        public string BackupName = "";
        public override string Description()
        {
            return "[群控][root]新建一个空的APP快照";
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
            return "创建APP快照";
        }
        public override bool ShowUserControl()
        {
            ModulePanel_Create modulePanel_Create = new ModulePanel_Create();
            modulePanel_Create.xmodules = GetXModules();
            Core.Client.ShowDialogPanel(modulePanel_Create);
            return true;

        }
        public override void Start()
        {
            if (PackageName.Length > 0)
            {
                if (BackupName.Trim().Length == 0)
                {
                    BackupName= DateTime.Now.ToString("yyMMddHHmmss");
                }
                Response res = Device.ScriptEngine.CreateSlot(PackageName, BackupName);
                if (!res.Result)
                {

                    Client.ShowToast("设备[" + Device.Name + "]无法创建快照[" + BackupName + "]" + res.Message, Color.FromRgb(239, 34, 7));

                }
                Console.WriteLine("APP快照创建结果：" + res.Json);
            }

        }
    }
    public class AppBackup_Manager : Core.XModule
    {
        public override string Description()
        {
            return "[群控][root]管理设备上的APP快照";
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
            return "APP快照管理";
        }
        public override bool ShowUserControl()
        {
            ModulePanel_Manager modulePanel_Manager = new ModulePanel_Manager();
            modulePanel_Manager.xmodules= GetXModules();
            Core.Client.ShowDialogPanel(modulePanel_Manager);
            return true;
        }
        public override void Start()
        {

        }
    }
}
