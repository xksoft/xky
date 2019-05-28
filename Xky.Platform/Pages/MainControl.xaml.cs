using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Model;
using Xky.Core.UserControl;

namespace Xky.Platform.Pages
{
    /// <summary>
    ///     MyMainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainControl : System.Windows.Controls.UserControl
    {
        private Thread _lastConnectThread;
        private readonly List<Device> _screenTickList = new List<Device>();

        public MainControl()
        {
            InitializeComponent();
            Common.MyMainControl = this;

            // SearchText.TextChanged += SearchText_TextChanged;
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Client.SearchDevices(SearchText.Text, GetTag(), GetDevicesByTag());
            if (!string.IsNullOrEmpty(SearchText.Text))
            {
                SearchResultLabel.Visibility = Visibility.Visible;
                SearchResultLabel.Foreground = Client.PanelDevices.Count > 0
                    ? new SolidColorBrush(Colors.Lime)
                    : new SolidColorBrush(Color.FromRgb(254, 65, 53));
                SearchResultLabel.Text = "找到" + Client.PanelDevices.Count + "台设备";
            }
            else
            {
                SearchResultLabel.Visibility = Visibility.Collapsed;
            }
        }

        private List<Device> GetDevicesByTag()
        {
            if (TagListBox.SelectedItem is Tag tag)
            {
                return tag.Devices;
            }

            var find = Client.Tags.ToList().Find(p => p.Name == "所有设备");
            if (find != null)
            {
                Console.WriteLine("设为选中");
                TagListBox.SelectedItem = find;
                return find.Devices;
            }

            return new List<Device>();
        }

        private string GetTag()
        {
            if (TagListBox.SelectedItem is Tag tag)
            {
                return tag.Name;
            }

            return "所有设备";
        }


        /// <summary>
        ///     加载设备列表
        /// </summary>
        public void LoadDevices()
        {
            Client.StartAction(() =>
            {
                while (true)
                {
                    Common.ShowToast("正在加载设备列表...");
                    var response = Client.LoadDevices();
                    if (response.Result)
                    {
                        Common.UiAction(() =>
                        {
                            DeviceListBox.ItemsSource = Client.PanelDevices;
                            TagListBox.ItemsSource = Client.Tags;
                            NodeTagListBox.ItemsSource = Client.NodeTags;
                            TagListBox.SelectedIndex = 0;
                        });
                        Common.ShowToast("设备加载成功");
                        break;
                    }

                    Common.ShowToast(response.Message);
                    Thread.Sleep(1000);
                }
            });
        }

        public void LoadNodes()
        {
            Client.StartAction(() =>
            {
                var response = Client.LoadNodes();
                Client.SearchLocalNode();


            });
        }

        /// <summary>
        ///     加载模块面板上的模块列表
        /// </summary>
        public void LoadModules()
        {
            Client.StartAction(() =>
            {
                Client.LoadModules();

                Console.WriteLine("成功加载模块：" + Client.Modules.Count + "个");
                Common.UiAction(() =>
                {
                    var view = CollectionViewSource.GetDefaultView(Client.Modules);
                    view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
                    ModuleListBox.ItemsSource = view;

                });
            }, ApartmentState.STA);
        }


        private void ScreenTick()
        {

            Client.StartAction(() =>
            {
                while (this.IsVisible)
                {
                    SendScrccnTick();
                    Thread.Sleep(10000);
                }
            });
        }

        private void SendScrccnTick()
        {
            IEnumerable<IGrouping<string, Device>> nodeGroup;
            lock ("getNodeGroup")
            {
                nodeGroup = from device in _screenTickList
                            group device by device.NodeSerial;
            }

            foreach (var group in nodeGroup)
            {
                Console.WriteLine("组:" + group.First().NodeSerial + " 量:" + group.Count());
                var sns = group.Select(device => device.Sn).ToList();
                var jarray = new JArray();
                foreach (var sn in sns)
                {
                    jarray.Add(sn);
                }

                Client.CallNodeEvent(group.First().NodeSerial, jarray,
                    new JObject { ["type"] = "send_screen", ["size"] = 0.3f, ["fps"] = 30 });
            }
        }

        #region ui事件

        private void DeviceListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                //上一个线程如果没完成就强制结束
                _lastConnectThread?.Abort();
                _lastConnectThread = Client.StartAction(() =>
                {
                    //加锁，避免出现线程安全问题
                    lock ("connect")
                    {
                        //连接屏幕
                        MyMirrorScreen.Connect(device);
                    }
                });
                RunningModules.ItemsSource = device.RunningModules;
                var rmmd5list = from rm in device.RunningModules select rm.Md5;
                foreach (var m in Client.Modules)
                {
                    if (rmmd5list.Contains(m.Md5))
                    {
                        m.State = 1;
                    }
                    else
                    {
                        m.State = 0;
                    }
                }


            }
            else
            {
                MyMirrorScreen.Disconnect();
            }
        }

        private void MyMirrorScreen_OnOnShowLog(object sender, string log, Color color)
        {
            Common.ShowToast(log, color);
        }

        private void Btn_back(object sender, RoutedEventArgs e)
        {
            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 4 });
            }
            else
            {
                MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 4 });
            }
        }

        private void Btn_home(object sender, RoutedEventArgs e)
        {

            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 3 });
            }
            else
            {
                MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 3 });
            }

        }

        private void Btn_task(object sender, RoutedEventArgs e)
        {

            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 187 });
            }
            else
            {

                MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 187 });
            }
        }

        private void Btn_RunningModule_Stop(object sender, RoutedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                if (sender is Button button)
                {
                    string modulemd5 = button.Tag.ToString();
                    StopModule(device, modulemd5);
                }
            }
        }

        private void RadioButton_ModuleTag_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MyModuleItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (MyModuleItem)((Border)e.Source).TemplatedParent;
            item.IsRunning = true;
            Client.StartAction(() =>
            {
                for (var i = 0; i < 5; i++) Thread.Sleep(1000);

                Dispatcher.Invoke(() => { item.IsRunning = false; });
            });
        }

        private void DeviceListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            lock ("getNodeGroup")
            {
                _screenTickList.Clear();
                _screenTickList.AddRange(Client.PanelDevices.ToList()
                    .GetRange(Convert.ToInt32(e.VerticalOffset), Convert.ToInt32(e.ViewportHeight)));
            }

            //发送一次心跳
            Client.StartAction(SendScrccnTick);
        }


        private void DeviceMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                string tag = ((MyImageButton)e.Source).Tag.ToString();
                switch (tag)
                {
                    case "GetDeviceDebug":
                        {
                            Client.StartAction(() =>
                            {
                                var response = Client.CallApi("get_device_debug", new JObject { ["sn"] = device.Sn });
                                if (response.Result)
                                {
                                    Console.WriteLine("界面元素授权码：" + response.Json["uispy_token"].ToString());
                                    Client.ShowToast("成功获取界面元素授权码，粘贴到调试工具中即可使用！", Color.FromRgb(0, 188, 0));
                                    try
                                    {
                                        System.Windows.Clipboard.SetDataObject(response.Json["uispy_token"].ToString(), true);
                                    }
                                    catch (Exception error) {
                                        Common.ShowToast("无法将界面元素授权码复制到当前系统粘贴板中，请关闭软件使用右键管理员权限启动！", Color.FromRgb(239, 34, 7));

                                    }
                                }
                                else
                                {
                                    Common.ShowToast(response.Message, Color.FromRgb(239, 34, 7));
                                }
                            }, ApartmentState.STA);
                            break;
                        }
                    case "ShowInputMethod":
                        {
                            device.ScriptEngine.ShowInputMethod();
                            break;
                        }
                    case "StopAllModules":
                        {
                            var md5list = from m in device.RunningModules.ToList() select m.Md5;
                            foreach (string md5 in md5list)
                            {
                                StopModule(device, md5);
                            }
                            Client.ShowToast("成功停止设备[" + device.Name + "]正在运行的所有模块！", Color.FromRgb(0, 188, 0));
                            break;
                        }
                    case "BackgroundApps": {
                            Client.StartAction(() =>
                            {

                                device.ScriptEngine.PressKey(187);
                            });
                            break;
                        }

                }
            }
        }

        private void MainControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            //开启屏幕小图心跳
            ScreenTick();
        }

        private void ModuleListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || e.ClickCount != 2)
            {
                return;
            }

            var module_select = (Module)ModuleListBox.SelectedItem;
            if (module_select != null)
            {

                if (DeviceListBox.SelectedItem is Device device)
                {
                    RunModule(device, module_select);
                }
            }
        }

        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((MenuItem)sender).Tag.ToString();
            switch (tag)
            {
                case "Reboot": {
                        if (DeviceListBox.SelectedItem is Device device)
                        {
                            Client.StartAction(() =>
                            {
                                device.ScriptEngine.AdbShell("reboot");
                            });
                        }
                        break;
                    }
                case "EditInfo":
                    {
                        string oldtag = "";
                        if (DeviceListBox.SelectedItem is Device device)
                        {
                            ContentControl_EditInfo_Tags.ItemsSource =
                                Client.Tags.ToList().FindAll(p => p.Name != "所有设备" && p.Name != "未分组设备");
                            TextBox_DeviceName.Text = device.Name;
                            if (device.Tags.Length > 0)
                            {
                                oldtag = device.Tags[0];
                                TextBox_DeviceTag.Text = device.Tags[0];
                            }
                            else
                            {
                                TextBox_DeviceTag.Text = "";
                            }

                            MyMessageBox msg =
                                new MyMessageBox(MessageBoxButton.YesNo, text_yes: "保存修改", text_no: "取消")
                                { MessageText = "" };
                            ((ContentControl)((Border)msg.Content).FindName("ContentControl")).Content =
                                ContentControl_EditInfo.Content;
                            Common.ShowMessageControl(msg);
                            if (msg.Result == MessageBoxResult.Yes)
                            {
                                string DeviceName = TextBox_DeviceName.Text;
                                string DeviceTag = TextBox_DeviceTag.Text;
                                Client.StartAction(() =>
                                {
                                    string[] tags = new string[0];
                                    if (DeviceTag.Length > 0)
                                    {
                                        tags = new string[] { DeviceTag };
                                    }

                                    Response response = Client.SetDevice(device.Sn, DeviceName, device.Description, tags);
                                    if (response.Result)
                                    {
                                        Common.ShowToast(response.Message, Color.FromRgb(0, 188, 0));
                                        device.Name = DeviceName;

                                        device.Tags = tags;
                                        if (oldtag != DeviceTag)
                                        {
                                            if (oldtag.Length > 0)
                                            {
                                                Client.RemoveTags(oldtag, device);
                                            }
                                            else
                                            {
                                                Client.RemoveTags("未分组设备", device);
                                            }

                                            if (DeviceTag.Length > 0)
                                            {
                                                Client.AddTags(DeviceTag, device);
                                            }
                                            else
                                            {
                                                Client.AddTags("未分组设备", device);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Common.ShowToast(response.Message, Color.FromRgb(239, 34, 7));
                                    }
                                });
                            }
                        }

                        break;
                    }
            }
        }

        private void Label_EditInfo_Tags_Click(object sender, RoutedEventArgs e)
        {
            TextBox_DeviceTag.Text = ((Label)sender).Content.ToString();
        }

        private void TagListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TagListBox.SelectedItem is Tag tag)
            {
                Client.BatchControlTag = tag;
                Client.SearchDevices(SearchText.Text, tag.Name, GetDevicesByTag());
            }
        }

        private void RunModule(Device device, Module rmodule)
        {

            var module = (Module)rmodule.Clone();
            XModule xmodule = (XModule)module.XModule.Clone();
            if (Client.BatchControl)
            {
                xmodule.Devices = Client.BatchControlTag.Devices;
            }
            //var runningmodule = device.RunningModules.ToList().Find(p => p.Md5 == module.Md5);
            //if (runningmodule != null)
            //{
            //    Common.ShowToast("设备["+device.Name+"]正在执行模块[" + runningmodule.Name + "]中，无法重复运行！", Color.FromRgb(239, 34, 7));
            //    return;
            //}

            //if (!xmodule.IsBackground())
            //{
            //    //如果是前台模块，同一时间只允许运行一个
            //    runningmodule = device.RunningModules.ToList().Find(p => p.XModule.IsBackground() == false);
            //    if (runningmodule != null)
            //    {
            //        Common.ShowToast("设备[" + device.Name + "]前台模块[" + runningmodule.Name + "]正在运行中，无法同时执行两个前台模块！",
            //            Color.FromRgb(239, 34, 7));
            //        return;
            //    }
            //}

            var thread = Client.StartAction(() =>
            {
                //xmodule.Device = device;
                //显示自定义控件
                var isContinue = false;
                Common.UiAction(() => { isContinue = xmodule.ShowUserControl(); }, false);

                //是否继续
                if (isContinue)
                {
                    var xmodules = xmodule.GetXModules();
                    foreach (var runmodule in xmodules)
                    {
                        Client.StartAction(() =>
                                          {
                                              runmodule.Start();
                                          });
                    }

                    //Dispatcher.Invoke(() => { device.RunningModules.Add(module); });
                    //xmodule.Start();

                    //Console.WriteLine("设备[" + device.Name + "]成功执行模块[" + module.Name + "]");

                    //Dispatcher.Invoke(() =>
                    //{
                    //    device.RunningModules.Remove(module);

                    //    if (device.RunningThreads.ContainsKey(module.Md5))
                    //    {
                    //        device.RunningThreads.Remove(module.Md5);
                    //    }
                    //});
                }
                //else
                //{
                //参数设置过程中取消执行
                //if (device.RunningThreads.ContainsKey(module.Md5))
                //{
                //    device.RunningThreads.Remove(module.Md5);
                //}
                // }
                //Dispatcher.Invoke(() =>
                //{
                //    if (DeviceListBox.SelectedItem is Device device_selected)
                //    {
                //        if (device_selected.Id == device.Id)
                //        {
                //            rmodule.State = 0;
                //        }

                //    }
                //});
            }, ApartmentState.STA);
            //if (!device.RunningThreads.ContainsKey(module.Md5))
            //{
            //    device.RunningThreads.Add(module.Md5, thread);
            //    rmodule.State = 1;
            //}
            //else
            //{
            //    Common.ShowToast("设备[" + device.Name + "]模块[" + runningmodule.Name + "]已经在运行中！",
            //        Color.FromRgb(239, 34, 7));
            //}
        }
        private void StopModule(Device device, string modulemd5) {
            if (device.RunningThreads.ContainsKey(modulemd5))
            {
                device.RunningThreads[modulemd5].Abort();

                device.RunningThreads.Remove(modulemd5);
                var runningmodule = device.RunningModules.ToList().Find(p => p.Md5 == modulemd5);
                if (runningmodule != null)
                {
                    device.RunningModules.Remove(runningmodule);
                    Client.Modules.ToList().Find(p => p.Md5 == modulemd5).State = 0;
                }
            }

        }
        private void Button_Module_Run_Click(object sender, RoutedEventArgs e)
        {
            string modulemd5 = ((MyButton)sender).Tag.ToString();
            var module =Client.Modules.ToList().Find(p=>p.Md5==modulemd5);
            if (module != null)
            {

                if (DeviceListBox.SelectedItem is Device device)
                {
                    RunModule(device, module);
                }
            }
        }

        private void Button_Module_Stop_Click(object sender, RoutedEventArgs e)
        {
            string modulemd5 = ((MyButton)sender).Tag.ToString();
            if (DeviceListBox.SelectedItem is Device device)
            {
                
                    
                    StopModule(device, modulemd5);
                
            }
        }

       
        private void TextBlock_ModulePanelNoData_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((TextBlock)sender).Name == "TextBlock_OpenModulePath")
            {
                System.Diagnostics.Process.Start(Client.ModulePath);
            }
            else
            {
                Common.OpenUrl("http://doc.xky.com");
            }

        }

        private void CheckBox_QK_Checked(object sender, RoutedEventArgs e)
        {
           
                Client.BatchControl = CheckBox_QK.IsChecked.Value;
            
        }
    }
}