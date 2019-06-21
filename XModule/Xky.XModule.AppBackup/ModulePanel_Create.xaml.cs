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
        public List<Core.XModule> xmodules = new List<Core.XModule>();
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
                ComboBox_List.Items.Add("正在加载APP列表...");
                ComboBox_List.SelectedIndex=0;
                Button_OK.IsEnabled = Button_Cancel.IsEnabled =ComboBox_List.IsEnabled= false;
                
            }));
                List<string> list = new List<string>();
            Response res_system  = xmodules[0].Device.ScriptEngine.AdbShell("pm list package -3");
            if (res_system.Json["result"] != null)
            {
                List<string> res = res_system.Json["result"].ToString().Split('\n').ToList();
                foreach (string s in res)
                {
                    if (s.StartsWith("package:"))
                    {
                       
                       int index = s.IndexOf("package:") + 8;
                       string PackageName = s.Substring(index, s.Length - index).Trim();
                       
                        list.Add(PackageName);
                    }
                }

            }
           
            this.Dispatcher.Invoke(new Action(() =>
            {
                ComboBox_List.Items.Clear();
               
                ComboBox_List.ItemsSource = list;
                if (ComboBox_List.Items.Count>0) { ComboBox_List.SelectedIndex = 0; }
                Button_OK.IsEnabled = Button_Cancel.IsEnabled = ComboBox_List.IsEnabled = true;
            }));


        }
        public void LoadBackups(string packagename)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Label_Backups_Loading.Content = "正在加载现有快照信息，请稍后...";
            }));
            List<Model.Backup> list = new List<Model.Backup>();
            Response res = xmodules[0].Device.ScriptEngine.GetSlotList(packagename);
            Response res_current = xmodules[0].Device.ScriptEngine.GetSlot(packagename);
            if (res.Result)
            {

                if (res.Json["list"] != null)
                {
                    foreach (var b in res.Json["list"])
                    {
                        Model.Backup backup = new Model.Backup();
                        backup.Name = b.ToString();
                        if (res_current.Json["name"] != null && res_current.Json["name"].ToString() == backup.Name)
                        {
                            backup.IsCurrent = true;

                        }
                        list.Add(backup);
                    }
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    BackupListBox.ItemsSource = list;
                    Label_Backups_Loading.Content = "当前设备上存在该APP的" + list.Count + "个快照";
                }));
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Label_Backups_Loading.Content = res.Message;
                }));
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Label_Tip.Text = "提示：创建快照会占用设备存储空间，当前设备["+ xmodules[0].Device.Name+ "]存储空间剩余["+(100-xmodules[0].Device.DiskUseage)+"%"+"]";
            new Thread(() =>
            {
               
                LoadPackages();
                
            })
            { IsBackground = true }.Start();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_List.SelectedItem != null) {
                foreach (var module in xmodules)
                {

                    ((AppBackup_Create)module).PackageName = ComboBox_List.SelectedItem.ToString();
                    ((AppBackup_Create)module).BackupName = TextBox_Name.Text;
                }
            }
            Client.CloseDialogPanel();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }

        private void ComboBox_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_List.SelectedItem != null&&ComboBox_List.SelectedItem.ToString()!= "正在加载APP列表...")
            {
                string pn = ComboBox_List.SelectedItem.ToString();
                new Thread(() =>
                {

                    LoadBackups(pn);

                })
                { IsBackground = true }.Start();
            }
        }
    }
}
