using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xky.Core;

namespace Xky.XModule.Instagram
{
    public class Instagram_打开APP : Core.XModule
    {
        public override string Name()
        {
            return "打开Instagram";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Instagram.logo.png");
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
        public override string Description()
        {
            return "打开Instagram";
        }

        public override void Start()
        {
            Client.Log("打开大萨达所大撒大","dsa",1);
          
            Device.ScriptEngine.AdbShell("am start com.instagram.android/com.instagram.android.activity.MainTabActivity");
           
            Device.ScriptEngine.Toast(Name()+"模块执行完毕", 1);
        }



        public override bool ShowUserControl()
        {
            return true;

        }
    }
}

