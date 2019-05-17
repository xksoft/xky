using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xky.XModule.Test
{
    public class Test : Core.XModule
    {
        public override string Name()
        {
            return "死循环测试模块";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.icon1.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[后台模块]死循环测试模块";
        }

        public override void Start()
        {
           while(true)
            {
                Device.ScriptEngine.Toast("死循环测试模块运行中");
                Thread.Sleep(5000);
            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class Test2 : Core.XModule
    {
        public override string Name()
        {
            return "死循环测试模块2";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.icon2.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[后台模块]死循环测试模块";
        }

        public override void Start()
        {
            while (true)
            {
                Device.ScriptEngine.Toast("死循环测试模块运行中");
                Thread.Sleep(5000);
            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class Test3 : Core.XModule
    {
        public override string Name()
        {
            return "死循环测试模块3";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.icon3.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[后台模块]死循环测试模块";
        }

        public override void Start()
        {
            while (true)
            {
                Device.ScriptEngine.Toast("死循环测试模块运行中");
                Thread.Sleep(5000);
            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class Test4 : Core.XModule
    {
        public override string Name()
        {
            return "死循环测试模块4";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.icon4.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[后台模块]死循环测试模块";
        }

        public override void Start()
        {
            while (true)
            {
                Device.ScriptEngine.Toast("死循环测试模块运行中");
                Thread.Sleep(5000);
            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class Test5 : Core.XModule
    {
        public override string Name()
        {
            return "死循环测试模块5";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.icon5.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[后台模块]死循环测试模块";
        }

        public override void Start()
        {
            while (true)
            {
                Device.ScriptEngine.Toast("死循环测试模块运行中");
                Thread.Sleep(5000);
            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
    public class Test6 : Core.XModule
    {
        public override string Name()
        {
            return "死循环测试模块6";
        }
        public override bool IsBackground()
        {
            return true;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Test.icon.icon6.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[后台模块]死循环测试模块";
        }

        public override void Start()
        {
            while (true)
            {
                Device.ScriptEngine.Toast("死循环测试模块运行中");
                Thread.Sleep(5000);
            }

        }
        public override bool ShowUserControl()
        {
            return true;

        }


    }
   
}
