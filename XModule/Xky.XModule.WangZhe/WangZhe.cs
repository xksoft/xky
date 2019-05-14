using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.WangZhe
{
    public class WangZhe : Core.XModule
    {
        public override string Name()
        {
            return "王者中单";
        }
        public override bool IsBackground()
        {
            return false;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.WangZhe.logo.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "王者中单，不屈白银";
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
