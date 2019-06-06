using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xky.Core.UserControl;

namespace Xky.XModule.AppBackup
{
    /// <summary>
    /// ModulePanel_Manager.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel_Manager : UserControl
    {
        ObservableCollection<Model.BDevice> list = new ObservableCollection<Model.BDevice>();
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

            list = new ObservableCollection<Model.BDevice>();
            foreach (var module in xmodules)
            {
                Model.BDevice bDevice = new Model.BDevice();
                bDevice.Device_Name = module.Device.Name;
                bDevice.Device_Sn = module.Device.Sn;
                Response res = module.Device.ScriptEngine.GetSlotList(packagename);
                Response res_current = module.Device.ScriptEngine.GetSlot(packagename);
                if (res.Json["list"] != null)
                {
                    foreach (var b in res.Json["list"])
                    {
                        Model.Backup backup = new Model.Backup();
                        backup.Name = b["name"].ToString();
                        if (res_current.Json["name"] != null && res_current.Json["name"].ToString() == backup.Name)
                        {
                            backup.IsCurrent = true;
                        }
                        backup.Device_Sn = module.Device.Sn;
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
                    ContentControl_MessageBox.Content = ContentControl_Loading.Content;
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
            if (ComboBox_List.SelectedItem != null && ComboBox_List.SelectedItem.ToString() != "请选择APP包名" && ComboBox_List.SelectedItem.ToString() != "正在加载APP列表...")
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

        private void Button_SetSlot_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_List.SelectedItem != null && ComboBox_List.SelectedItem.ToString() != "请选择APP包名" && ComboBox_List.SelectedItem.ToString() != "正在加载APP列表...")
            {
                string packagename = ComboBox_List.SelectedItem.ToString();
                Model.Backup backup = (Model.Backup)((MyImageButton)sender).Tag;
                var device = from m in xmodules where m.Device.Sn == backup.Device_Sn select m.Device;
                if (device != null)
                {
                    Device dc = (Device)device.First();
                    new Thread(() =>
                    {
                        ShowLoading("设备[" + dc.Name + "]正在切换到快照[" + backup.Name + "]，请稍后...");
                        Response res = dc.ScriptEngine.SetSlot(packagename, backup.Name);
                        if (res.Result)
                        {
                            Client.ShowToast("设备[" + dc.Name + "]成功切换到快照[" + backup.Name + "]", Color.FromRgb(0, 188, 0));
                            var dDevice = list.ToList().Find(p => p.Device_Sn == dc.Sn);
                            foreach (var b in dDevice.Backups)
                            {
                                b.IsCurrent = false;
                            }
                            if (dDevice != null)
                            {
                                dDevice.Backups.ToList().Find(p => p.Name == backup.Name).IsCurrent = true;
                            }
                            dDevice.Backups = dDevice.Backups;
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                BDeviceListBox.ItemsSource = null;
                                BDeviceListBox.ItemsSource = list;

                            }));
                        }
                        else
                        {
                            Client.ShowToast("设备[" + dc.Name + "]无法切换到快照[" + backup.Name + "]" + res.Message, Color.FromRgb(239, 34, 7));

                        }
                        CloseLoading();

                    })
                    { IsBackground = true }.Start();
                }
            }
        }
        Model.Backup backup_delete;
        private void Button_DeleteSlot_Click(object sender, RoutedEventArgs e)
        {
           
            ContentControl_MessageBox.Content = ContentControl_Delete.Content;
            Grid_MessageBox.Visibility = Visibility.Visible;


            if (ComboBox_List.SelectedItem != null && ComboBox_List.SelectedItem.ToString() != "请选择APP包名" && ComboBox_List.SelectedItem.ToString() != "正在加载APP列表...")
            {
                string packagename = ComboBox_List.SelectedItem.ToString();
                backup_delete = (Model.Backup)((MyImageButton)sender).Tag;
                var device = from m in xmodules where m.Device.Sn == backup_delete.Device_Sn select m.Device;
                if (device != null)
                {
                    Device dc = (Device)device.First();
                    Label_Tip_Delete.Content = "确定要删除设备["+dc.Name+"]上的快照["+backup_delete.Name+"]吗？";
                }
            }
            else
            {

                Grid_MessageBox.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Delete_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Grid_MessageBox.Visibility = Visibility.Collapsed;
        }

        private void Button_Delete_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_List.SelectedItem != null && ComboBox_List.SelectedItem.ToString() != "请选择APP包名" && ComboBox_List.SelectedItem.ToString() != "正在加载APP列表...")
            {
                string packagename = ComboBox_List.SelectedItem.ToString();
                var device = from m in xmodules where m.Device.Sn == backup_delete.Device_Sn select m.Device;
                if (device != null)
                {
                    Device dc = (Device)device.First();
                    new Thread(() =>
                    {
                        ShowLoading("正在从设备[" + dc.Name + "]移除快照[" + backup_delete.Name + "]，请稍后...");
                        Response res = dc.ScriptEngine.DelSlot(packagename, backup_delete.Name);
                        if (res.Result)
                        {
                            Client.ShowToast("成功从设备[" + dc.Name + "]移除快照[" + backup_delete.Name + "]", Color.FromRgb(0, 188, 0));
                            var dDevice = list.ToList().Find(p => p.Device_Sn == dc.Sn);
                          
                           
                            this.Dispatcher.Invoke(new Action(() =>
                            { dDevice.Backups.Remove(backup_delete);
                                BDeviceListBox.ItemsSource = null;
                                BDeviceListBox.ItemsSource = list;

                            }));
                        }
                        else
                        {
                            Client.ShowToast("设备[" + dc.Name + "]无法切换到快照[" + backup_delete.Name + "]" + res.Message, Color.FromRgb(239, 34, 7));

                        }
                        CloseLoading();
                    })
                    { IsBackground = true }.Start();
                }
            }
        }
    }
}
