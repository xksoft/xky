using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.Platform.UserControl.Pages
{
    /// <summary>
    ///     MyMainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyMainControl : System.Windows.Controls.UserControl
    {
        private Thread _lastConnectThread;

        public MyMainControl()
        {
            InitializeComponent();
            Common.MyMainControl = this;
        }


        /// <summary>
        ///     加载设备列表
        /// </summary>
        public void LoadDevices()
        {
            Client.StartAction(() =>
            {
                Common.ShowToast("正在加载设备列表...");

                var response = Client.LoadDevices();
                if (response.Result)
                {
                    Console.WriteLine("设备数：" + Client.Devices.Count);
                    Common.UiAction(() => { DeviceListBox.ItemsSource = Client.Devices; });

                    Common.ShowToast("设备加载成功");
                }
                else
                {
                    Common.ShowToast(response.Message);
                }
            });
        }

        /// <summary>
        ///     加载模块面板上的模块列表
        /// </summary>
        public void LoadModules_Panel()
        {
            Client.StartAction(() =>
            {
                var response = Client.LoadModules_Panel();
                if (response.Result)
                {
                    Console.WriteLine("模块面板上的模块数量：" + Client.Modules_Panel.Count);
                    Common.UiAction(() => { ModulesPanel.ItemsSource = Client.Modules_Panel; });
                    Common.UiAction(() => { ModulesTagsPanel.ItemsSource = Client.Modules_Panel_Tags; });
                    Common.ShowToast("模块面板加载成功");
                }
                else
                {
                    Common.ShowToast(response.Message);
                }
            });
        }

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
            }
        }

        private void MyMirrorScreen_OnOnShowLog(object sender, string log, Color color)
        {
            Common.ShowToast(log, color);
        }

        private void Btn_back(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject {["type"] = "device_button", ["name"] = "code", ["key"] = 4});
        }

        private void Btn_home(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject {["type"] = "device_button", ["name"] = "code", ["key"] = 3});
        }

        private void Btn_task(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject {["type"] = "device_button", ["name"] = "code", ["key"] = 187});
        }

        private void RadioButton_ModuleTag_Click(object sender, RoutedEventArgs e)
        {
            var btn = (RadioButton) e.Source;
            if (btn.IsChecked.Value)
            {
                if (btn.Tag.ToString() == "所有模块")
                    Common.UiAction(() => { ModulesPanel.ItemsSource = Client.Modules_Panel; });
                else
                    Common.UiAction(() =>
                    {
                        ModulesPanel.ItemsSource = from module in Client.Modules_Panel
                            where module.Tags.Contains(btn.Tag.ToString())
                            select module;
                    });
            }
        }

        private void MyModuleItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (MyModuleItem) ((Border) e.Source).TemplatedParent;
            item.IsRunning = true;
            Client.StartAction(() =>
            {
                for (var i = 0; i < 5; i++) Thread.Sleep(1000);

                Dispatcher.Invoke(() => { item.IsRunning = false; });
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var response = Client.CallApi("get_user", new JObject());
            Console.WriteLine(response.Json);
        }
    }
}