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

namespace Xky.XModule.Adb
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

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }
        private void Button_Cmd_Click(object sender, RoutedEventArgs e) {

            string cmd = ((MyButton)sender).Tag.ToString();
            if (cmd.StartsWith("[adbshell]"))
            {
                CheckBox_Shell.IsChecked=true;
                TextBox_Cmd.Text=cmd.Substring(cmd.IndexOf("[adbshell]") + 10);
            }
            else
            {
                CheckBox_Shell.IsChecked = false;
                TextBox_Cmd.Text = cmd;
            }
        }
        private void Button_Run_Click(object sender, RoutedEventArgs e)
        {
            RunCmd(TextBox_Cmd.Text);
        }
        public void RunCmd(string cmd)
        {
            //Response res;
            //if (CheckBox_Shell.IsChecked.Value)
            //{
            //    res= device.ScriptEngine.AdbShell(cmd);
            //}
            //else
            //{
            //     res  = device.ScriptEngine.AdbCommand(cmd);
            //}
            //if (res != null && res.Result&&res.Json!=null&&res.Json["result"]!=null)
            //{
            //    TextBox_Result.Text = res.Json["result"].ToString();
            //}
            //else
            //{
            //    TextBox_Result.Text = "执行失败！";

            //}

        }

        private void Button_SetClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetDataObject(TextBox_Cmd.Text, true);
            }
            catch { }
        }
    }
}
