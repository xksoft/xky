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
    public class TestModule : Core.XModule
    {
        public override string Name()
        {
            return "我是模块的名字";
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.Demo.icon.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);

            //  var decoder = BitmapDecoder.Create(ms,                      
            //BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);                       
            //var imageSource = new BitmapImage();
            //imageSource.BeginInit();
            //imageSource.StreamSource = myStream;
            //imageSource.EndInit();

            // Assign the Source property of your image      
            //return imageSource;
            return bytes;

        }
        public override string Description()
        {
            return "我是模块的描述";
        }

        public override void Start()
        {
            Device.ScriptEngine.Toast("用户名：" + MyStr, 1);
            for (int i = 20; i >0; i--)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i + "秒后继续执行");
                Console.WriteLine("当前设备ID："+Device.Id);
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
}