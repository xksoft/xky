using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Xky.XModule.Demo
{
    public class Class1 : Core.XModule
    {
        public override string Name()
        {
            return "aaa";
        }

        public override string Description()
        {
            return "aaa的描述";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("打声招呼 " + mystr, 1);
        }

        public string mystr = "时间";


        public override bool ShowUserControl()
        {
            var aaa = new UserControl1();
            Core.Client.ShowDialogPanel(aaa);
            mystr = aaa.txtvalue.Text;
            return true;
        }
    }
}