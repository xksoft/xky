using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Xky.XModule.ContactManager
{
    public class ContactManager : Core.XModule
    {
        public int t = 0;
        public override string Description()
        {
            return "[支持群控]清空、导入联系人";
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
            return "联系人管理";
        }
       
        public override bool ShowUserControl()
        {
            
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
