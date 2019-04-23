using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Xky.XModule.Demo
{
    public class TestModule1 : Core.XModule
    {
        public override string Name()
        {
            return "显示设备ID";
        }
        public override bool IsBackground()
        {
            return false;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon1.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[前台模块]该模块主要用来显示设备ID";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 10; i >0; i--)
            {
                Thread.Sleep(5000);
                Device.ScriptEngine.Toast("当前设备ID："+Device.Id, 1);
            }
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }

        public string MyStr = "测试字符";
     

        public override bool ShowUserControl()
        {
         
            var panel = new ModulePanel();
          
            //ShowDialogPanel会有类似ShowDialog的效果，堵塞线程等待关闭后继续执行
            Core.Client.ShowDialogPanel(panel);
            //关闭后赋值
            MyStr = panel.text_username.Text;
            //返回true让模块继续执行，否则会直接结束
            Console.WriteLine("模块调用返回："+panel.run);
            return panel.run;
        }
    }
    public class TestModule2 : Core.XModule
    {
        public override string Name()
        {
            return "显示内存使用率";
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon2.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "[前台模块]该模块主要用来显示内存使用率";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 10; i > 0; i--)
            {
                Thread.Sleep(5000);
                Device.ScriptEngine.Toast("当前设备内存使用：" + Device.MemoryUseage, 1);
            }
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }

        public string MyStr = "测试字符";

        public override bool IsBackground() {
            return false;
        }
        public override bool ShowUserControl()
        {

            var panel = new ModulePanel();

            //ShowDialogPanel会有类似ShowDialog的效果，堵塞线程等待关闭后继续执行
            Core.Client.ShowDialogPanel(panel);
            //关闭后赋值
            MyStr = panel.text_username.Text;
            //返回true让模块继续执行，否则会直接结束
            Console.WriteLine("模块调用返回：" + panel.run);
            return panel.run;
        }
    }
    public class TestModule3 : Core.XModule
    {
        public override string Name()
        {
            return "显示CPU使用率";
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon3.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "该模块主要用来显示CPU使用率";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 10; i > 0; i--)
            {
                Thread.Sleep(5000);
                Device.ScriptEngine.Toast("当前设备CPU使用：" + Device.CpuUseage, 1);
            }
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }

        public string MyStr = "测试字符";

        public override bool IsBackground()
        {
            return true;
        }
        public override bool ShowUserControl()
        {

            var panel = new ModulePanel();

            //ShowDialogPanel会有类似ShowDialog的效果，堵塞线程等待关闭后继续执行
            Core.Client.ShowDialogPanel(panel);
            //关闭后赋值
            MyStr = panel.text_username.Text;
            //返回true让模块继续执行，否则会直接结束
            Console.WriteLine("模块调用返回：" + panel.run);
            return panel.run;
        }
    }
    public class TestModule4 : Core.XModule
    {
        public override string Name()
        {
            return "显示手机存储可用空间率";
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon4.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "我是模块的描述显示手机存储可用空间率";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 10; i > 0; i--)
            {
                Thread.Sleep(5000);
                Device.ScriptEngine.Toast("当前设备存储使用：" + Device.DiskUseage, 1);
            }
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }

        public string MyStr = "测试字符";

        public override bool IsBackground()
        {
            return true;
        }
        public override bool ShowUserControl()
        {

            var panel = new ModulePanel();

            //ShowDialogPanel会有类似ShowDialog的效果，堵塞线程等待关闭后继续执行
            Core.Client.ShowDialogPanel(panel);
            //关闭后赋值
            MyStr = panel.text_username.Text;
            //返回true让模块继续执行，否则会直接结束
            Console.WriteLine("模块调用返回：" + panel.run);
            return panel.run;
        }
    }
    public class TestModule5 : Core.XModule
    {
        public override string Name()
        {
            return "显示设备厂商";
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon5.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "该模块主要用来显示设备厂商";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 10; i > 0; i--)
            {
                Thread.Sleep(5000);
                Device.ScriptEngine.Toast("当前设备厂商：" + Device.Product, 1);
            }
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }

        public string MyStr = "测试字符";

        public override bool IsBackground()
        {
            return true;
        }
        public override bool ShowUserControl()
        {

            var panel = new ModulePanel();

            //ShowDialogPanel会有类似ShowDialog的效果，堵塞线程等待关闭后继续执行
            Core.Client.ShowDialogPanel(panel);
            //关闭后赋值
            MyStr = panel.text_username.Text;
            //返回true让模块继续执行，否则会直接结束
            Console.WriteLine("模块调用返回：" + panel.run);
            return panel.run;
        }
    }
    public class TestModule6 : Core.XModule
    {
        public override string Name()
        {
            return "无参数模块";
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon6.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;

        }
        public override string Description()
        {
            return "该模块直接双击运行无需设置参数";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 10; i > 0; i--)
            {
                Thread.Sleep(5000);
                Device.ScriptEngine.Toast("当前设备厂商：" + Device.Product, 1);
            }
            Device.ScriptEngine.Toast("模块执行完毕", 1);
        }

        public string MyStr = "测试字符";

        public override bool IsBackground()
        {
            return false;
        }
        public override bool ShowUserControl()
        {

            return true;
        }
    }
}