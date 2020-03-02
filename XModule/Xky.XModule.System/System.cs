﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Xky.XModule.System
{
    public class System_Locked : XCore.XModule
    {
        public override string Name()
        {
            return "锁屏";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.System.icon.locked.png");
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
            return "[后台模块]锁定设备屏幕";
        }

        public override void Start()
        {
            Device.ScriptEngine.LockScreen();

        }
        public override bool ShowUserControl()
        {
            return true;

        }
    }
    public class System_UnLocked : XCore.XModule
    {
        public override string Name()
        {
            return "解锁";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.System.icon.unlocked.png");
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
            return "[后台模块]解锁设备屏幕";
        }

        public override void Start()
        {
            Device.ScriptEngine.AdbShell("input keyevent 26");
          

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class System_FastReboot : XCore.XModule
    {
        public override string Name()
        {
            return "软重启";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.System.icon.reboot.png");
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
            return "[后台模块]快速重启设备";
        }

        public override void Start()
        {
            var resp = Device.ScriptEngine.AdbShell("ps");
            var processlist = resp.Json["result"].ToString();
            Regex reg = new Regex("root.*zygote");
            MatchCollection mc = reg.Matches(processlist);
            foreach (Match m in mc)
            {
                reg = new Regex("(?<=root).+?\\d+");
                Match match = reg.Match(m.Value);
                if (match.Success)
                {
                    int pid = 0;
                    int.TryParse(match.Value, out pid);
                    if(pid>0)
                    {
                        Device.ScriptEngine.AdbShell("kill "+pid);
                    }
                }

            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class System_Reboot : XCore.XModule
    {
        public override string Name()
        {
            return "硬重启";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.System.icon.reboot.png");
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
            return "[后台模块]重启设备";
        }

        public override void Start()
        {
            Device.ScriptEngine.AdbCommand("reboot") ;


        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
   
}
  