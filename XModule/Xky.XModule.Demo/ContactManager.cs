using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Xky.XModule.ContactManager
{
    public class ContactManager : Core.XModule
    {
        public string username = "小明";
        public override string Description()
        {
            return "[支持群控]添加好友";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.ContactManager.icon.png");
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
            return "添加好友";
        }
       
        public override bool ShowUserControl()
        {
            List<string> userlist = new List<string>() { "小明","小红","二狗"};
            //获取到将要在每台设备上执行的模块实例
            List<Core.XModule> modulelist = GetXModules();
            for (int i=0;i<userlist.Count;i++)
            {
                if (modulelist.Count > i)
                {
                    //将模块实例强制转换为当前模块实例
                    ContactManager cm = (ContactManager)modulelist[i];
                    //修改全局变量的值
                    cm.username = userlist[i];

                  
                }
            }
            
             return true;
        }
        public override void Start()
        {
            Device.ScriptEngine.Toast(username);

        }
    }
}
