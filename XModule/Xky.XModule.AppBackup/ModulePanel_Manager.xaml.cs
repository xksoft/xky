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
    /// ModulePanel_Manager.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel_Manager : UserControl
    {
        public class BDevice
        {
            private string _device_Name = "";

            public string Device_Name { get => _device_Name; set => _device_Name = value; }
            public List<Backup> Backups { get => _backups; set => _backups = value; }

            private List<Backup> _backups = new List<Backup>();
        }
        public class Backup {
            private string _name = "";
            private bool _isCurrent = false;
            public string Name { get => _name; set => _name = value; }
            public bool IsCurrent { get => _isCurrent; set => _isCurrent = value; }
        }
        public List<Core.XModule> xmodules = new List<Core.XModule>();
        public ModulePanel_Manager()
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
                ComboBox_List.SelectedIndex = 0;
                ComboBox_List.IsEnabled = false;

            }));
            List<string> list = new List<string>();
            Response res_system = xmodules[0].Device.ScriptEngine.AdbShell("pm list package -3");
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
               
                ComboBox_List.Items.Clear();
                if (list.Count > 0)
                {
                    ComboBox_List.IsEnabled = true;
                    list.Add(list[0]);
                    list[0] = "请选择APP包名";
                    ComboBox_List.ItemsSource = list;
                    if (ComboBox_List.Items.Count > 0) { ComboBox_List.SelectedIndex = 0; }
                }
              
               
            }));


        }
        public void LoadAppBackups(string packagename)
        {

            List<BDevice> list = new List<BDevice>();
            foreach (var module in xmodules)
            {
                BDevice bDevice = new BDevice();
                bDevice.Device_Name = module.Device.Name;
                Response res= module.Device.ScriptEngine.GetSlotList(packagename);
                if (res.Json["list"]!=null)
                {
                    foreach (var b in res.Json["list"])
                    {
                        Backup backup = new Backup();
                        backup.Name = b["name"].ToString();
                        bDevice.Backups.Add(backup);
                    }
                }
                list.Add(bDevice);
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                BDeviceListBox.ItemsSource = list;
            }));
        }
        public void ShowLoading(string text)
        {
            if (Grid_MessageBox.Visibility == Visibility.Visible)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Label_Loading.Content = text;
                }));
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ContentControl_MessageBox.Content = ContentControl_Loading.Content;
                    Label_Loading.Content = text;
                    Grid_MessageBox.Visibility = Visibility.Visible;
                }));
            }
        }
        public void CloseLoading()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Grid_MessageBox.Visibility = Visibility.Hidden;
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

        private void ComboBox_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_List.SelectedItem!=null&&ComboBox_List.SelectedItem.ToString()!= "请选择APP包名"&&ComboBox_List.SelectedItem.ToString()!= "正在加载APP列表...")
            {
                string packagename = ComboBox_List.SelectedItem.ToString();
                new Thread(() =>
                {
                    ShowLoading("快照列表加载中，请稍后...");
                    LoadAppBackups(packagename);
                    CloseLoading();

                })
                { IsBackground = true }.Start();
            }
        }
    }
}
