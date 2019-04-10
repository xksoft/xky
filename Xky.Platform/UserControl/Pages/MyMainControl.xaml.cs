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
            SearchText.TextBox1.TextChanged += SearchText_TextChanged;
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = from d in Client.Devices
                where d.Name.Contains(SearchText.TextBox1.Text)
                orderby d.Name
                select d;
            var list = source.ToList();
            for (int i = 0; i < 100; i++)
            {
                list.AddRange(source.ToList());
            }

            DeviceListBox.ItemsSource = list;
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
                        Common.UiAction(() => { DeviceListBox.ItemsSource = Client.Devices; });
                        Common.ShowToast("设备加载成功");
                        break;
                    }
                    Common.ShowToast(response.Message);
                    Thread.Sleep(1000);
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
                    Console.WriteLine("模块面板上的模块数量：" + Client.ModulesPanel.Count);
                    Common.UiAction(() => { ModulesPanel.ItemsSource = Client.ModulesPanel; });
                    Common.UiAction(() => { ModulesTagsPanel.ItemsSource = Client.ModulesPanelTags; });
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
                    Common.UiAction(() => { ModulesPanel.ItemsSource = Client.ModulesPanel; });
                else
                    Common.UiAction(() =>
                    {
                        ModulesPanel.ItemsSource = from module in Client.ModulesPanel
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

        private void DeviceListBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void DeviceListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //var aaa=  DeviceListBox.Items.Cast<UIElement>().Where(x => x.IsVisible);
            Console.WriteLine("触发");
            Console.WriteLine("Visible Item Start Index:{0}", e.VerticalOffset);
            Console.WriteLine("Visible Item Count:{0}", e.ViewportHeight);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("1");
            if (DeviceListBox.SelectedItem is Device device)
            {
                Console.WriteLine("2");
                Client.StartAction(() => { Console.WriteLine(device.ScriptEngine.AdbShell("ls").Json); });
            }
        }
    }
}