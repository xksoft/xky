using System;
using System.Collections.Generic;
using System.Linq;
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
        private Device CurrentDevice;
        private readonly List<Device> _screenTickList = new List<Device>();

        public MainControl()
        {
            InitializeComponent();
            Common.MyMainControl = this;

            // SearchText.TextChanged += SearchText_TextChanged;


        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Client.SearchDevices(SearchText.Text);
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
                while (true)
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

            foreach (var zu in nodeGroup)
            {
                Console.WriteLine("组:" + zu.First().NodeSerial + " 量:" + zu.Count());
                var sns = zu.Select(device => device.Sn).ToList();
                var jarray = new JArray();
                foreach (var sn in sns)
                {
                    jarray.Add(sn);
                }
                Client.CallNodeEvent(zu.First().NodeSerial, jarray,
                    new JObject { ["type"] = "send_screen" });
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
            }
        }

        private void MyMirrorScreen_OnOnShowLog(object sender, string log, Color color)
        {
            Common.ShowToast(log, color);
        }

        private void Btn_back(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 4 });
        }

        private void Btn_home(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 3 });
        }

        private void Btn_task(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 187 });
        }
        private void Btn_RunningModule_Stop(object sender, RoutedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                if (sender is Button button)
                {
                    string module_md5 = button.Tag.ToString();
                    if (device.RunningThreads.ContainsKey(module_md5))
                    {
                        device.RunningThreads[module_md5].Abort();

                        device.RunningThreads.Remove(module_md5);
                        var runningmodule = device.RunningModules.ToList().Find(p => p.Md5 == module_md5);
                        if (runningmodule != null)
                        {
                            device.RunningModules.Remove(runningmodule);
                        }
                    }
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
                _screenTickList.AddRange(Client.Devices.ToList()
                    .GetRange(Convert.ToInt32(e.VerticalOffset), Convert.ToInt32(e.ViewportHeight)));
            }
            //发送一次心跳
            Client.StartAction(SendScrccnTick);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeviceMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            string tag = ((MyImageButton)e.Source).Tag.ToString();
            MessageBox.Show(tag);
        }

        #endregion

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
                var module = (Module)module_select.Clone();
                if (DeviceListBox.SelectedItem is Device device)
                {
                    XModule xmodule = (XModule)module.XModule.Clone();
                    var runningmodule = device.RunningModules.ToList().Find(p => p.Md5 == module.Md5);
                    if (runningmodule != null)
                    {
                        Common.ShowToast("该设备正在执行模块[" + runningmodule.Name + "]中，无法重复运行！", Color.FromRgb(239, 34, 7));
                        return;
                    }
                    if (!xmodule.IsBackground())
                    {
                        //如果是前台模块，同一时间只允许运行一个
                        runningmodule = device.RunningModules.ToList().Find(p => p.XModule.IsBackground() == false);
                        if (runningmodule != null)
                        {
                            Common.ShowToast("前台模块[" + runningmodule.Name + "]正在运行中，无法同时执行两个前台模块！", Color.FromRgb(239, 34, 7));
                            return;
                        }
                    }

                    var thread = Client.StartAction(() =>
                    {
                        //显示自定义控件
                        var isContinue = false;
                        Common.UiAction(() =>
                        {
                            isContinue = xmodule.ShowUserControl();
                        }, false);
                        //是否继续
                        if (isContinue)
                        {
                            xmodule.Device = device;
                            Dispatcher.Invoke(() =>
                            {
                                device.RunningModules.Add(module);


                            });
                            xmodule.Start();
                            Console.WriteLine("设备[" + device.Id + "]成功执行模块[" + module.Name + "]");
                            Dispatcher.Invoke(() =>
                            {
                                device.RunningModules.Remove(module);
                                if (device.RunningThreads.ContainsKey(module.Md5))
                                {
                                    device.RunningThreads.Remove(module.Md5);
                                }
                            });
                        }
                        else
                        {
                            //参数设置过程中取消执行
                            if (device.RunningThreads.ContainsKey(module.Md5))
                            {
                                device.RunningThreads.Remove(module.Md5);
                            }
                        }

                    }, ApartmentState.STA);
                    device.RunningThreads.Add(module.Md5, thread);

                }
            }
        }
    }
}