using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.Platform.UserControl.Pages
{
    /// <summary>
    /// MyMainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyMainControl : System.Windows.Controls.UserControl
    {
        public MyMainControl()
        {
            InitializeComponent();
            Common.MyMainControl = this;
        }


        /// <summary>
        /// 加载设备列表
        /// </summary>
        public void LoadDevices()
        {
            Client.StartAction(async () =>
            {
                Common.ShowToast("正在加载设备列表...");

                var response = await Client.LoadDevices();
                if (response.Result)
                {
                    Console.WriteLine("设备数：" + Client.Devices.Count);
                    Common.UiAction(() => { DeviceListBox.ItemsSource = Client.Devices; });

                    Common.ShowToast("设备加载成功");
                }
                else
                {
                    Common.ShowToast("设备加载失败");
                }
            });
        }
        /// <summary>
        /// 加载模块面板上的模块列表
        /// </summary>
        public void LoadModules_Panel()
        {
            Client.StartAction(async () =>
            {
               

                var response = await Client.LoadModules_Panel();
                if (response.Result)
                {
                    Console.WriteLine("模块面板上的模块数量：" + Client.Modules_Panel.Count);
                    Common.UiAction(() => { ModulesPanel.ItemsSource = Client.Modules_Panel; });
                    Common.UiAction(() => { ModulesTagsPanel.ItemsSource = Client.Modules_Panel_Tags; });
                    Common.ShowToast("模块面板加载成功");
                }
                else
                {
                    Common.ShowToast("模块面板加载失败");
                }
            });
        }

        private void DeviceListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                Client.StartAction(() =>
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
            MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 4 });
        }

        private void Btn_home(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject { ["type"] = "device_button", ["name"] = "code", ["key"] = 3});
        }

        private void Btn_task(object sender, RoutedEventArgs e)
        {
            MyMirrorScreen.EmitEvent(new JObject {["type"] = "device_button", ["name"] = "code", ["key"] = 187});
        }

        private void RadioButton_ModuleTag_Click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)e.Source;
            if (btn.IsChecked.Value) {
                if (btn.Tag.ToString() == "所有模块")
                {
                    Common.UiAction(() => { ModulesPanel.ItemsSource = Client.Modules_Panel; });
                }
                else { Common.UiAction(() => { ModulesPanel.ItemsSource = from module in Client.Modules_Panel where module.Tags.Contains(btn.Tag.ToString()) select module; }); }
              

            }
           
        }
    }
}