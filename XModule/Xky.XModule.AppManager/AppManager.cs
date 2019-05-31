using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.AppManager
{
    public class AppManager : Core.XModule
    {
        public override string Description()
        {
            return "[支持群控]管理手机上的应用";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.AppManager.icon.png");
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
            return "应用管理器";
        }

        public override void Start()
        {

        }
        public override bool ShowUserControl()
        {

            var panel = new ModulePanel();
            panel.devices = Devices;
            Core.Client.ShowDialogPanel(panel);
            return true;
        }
    }
}
