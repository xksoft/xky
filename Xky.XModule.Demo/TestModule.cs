using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Xky.XModule.Demo
{
    public class TestModule : Core.XModule
    {
        public override string Name()
        {
            return "我是模块的名字";
        }
       
        public override string Description()
        {
            return "我是模块的描述";
        }

        public override void Start()
        {
           Device.ScriptEngine.Toast("用户名：" + MyStr, 1); 
          
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