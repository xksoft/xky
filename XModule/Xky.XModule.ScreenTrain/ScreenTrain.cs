using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.ScreenTrain
{
    public class ScreenTrain : Core.XModule
    {
        public override string Description()
        {
            return "屏幕元素训练";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.ScreenTrain.icon.png");
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
            return "屏幕元素训练";
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
