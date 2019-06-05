using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.XModule.AppBackup
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel_Create : UserControl
    {
        public Device device;
        public ModulePanel_Create()
        {
            InitializeComponent();
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }
        public void LoadPackages()
        {

            this.Dispatcher.Invoke(new Action(() =>
            {
                Button_OK.IsEnabled = Button_Cancel.IsEnabled =ComboBox_List.IsEnabled= false;
                Label_Loading.Visibility = Visibility.Visible;
            }));
                List<string> list = new List<string>();
            Response res_system  = device.ScriptEngine.AdbShell("pm list package -3");
            if (res_system.Json["result"] != null)
            {
                List<string> res = res_system.Json["result"].ToString().Split('\n').ToList();
                foreach (string s in res)
                {
                    if (s.StartsWith("package:"))
                    {
                       
                       int index = s.IndexOf("package:") + 8;
                       string PackageName = s.Substring(index, s.Length - index);
                       
                        list.Add(PackageName);
                    }
                }

            }
           
            this.Dispatcher.Invoke(new Action(() =>
            {
                Label_Loading.Visibility = Visibility.Hidden;
                ComboBox_List.ItemsSource = list;
                if (ComboBox_List.Items.Count>0) { ComboBox_List.SelectedIndex = 0; }
                Button_OK.IsEnabled = Button_Cancel.IsEnabled = ComboBox_List.IsEnabled = true;
            }));


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
               
                LoadPackages();
                
            })
            { IsBackground = true }.Start();
        }
    }
}
