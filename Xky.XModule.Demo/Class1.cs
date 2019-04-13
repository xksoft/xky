using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.Demo
{
    public class Class1:Core.XModule
    {
        public override string Name()
        {
            return "aaa";
        }

        public override string Description()
        {
            return "aaa的描述";
        }

        public override void Action()
        {
           Console.WriteLine("bbbbb");
            Device.ScriptEngine.Toast("打声招呼",1);
        }
    }
}
