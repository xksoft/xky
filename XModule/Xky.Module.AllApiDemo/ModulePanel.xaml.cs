using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;
using Xky.Core.UserControl;

namespace Xky.XModule.AllApiDemo
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public ModulePanel()
        {
            InitializeComponent();
        }
        public Device device;
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((MyButton)sender).Tag.ToString();
            Response res = null ;
            switch (tag)
            {

                case "AdbShell":
                    {
                        res = device.ScriptEngine.AdbShell("ls sdcard");
                        break;
                    }
                case "Toast":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "FindMe":
                    {
                        res = device.ScriptEngine.FindMe();
                        break;
                    }
                case "WakeUp":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "LockScreen":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "InstallApkFromUrl":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "RestartApp":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "KillApp":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "ClearApp":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "3213123":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }

            }
            if (res!=null)
            {
                TextBox_Result.Text = res.Json.ToString();
            }
        }
    }
}
