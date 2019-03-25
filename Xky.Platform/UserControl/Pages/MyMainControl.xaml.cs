using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xky.Core;

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

        public void LoadDevices()
        {
            Task.Factory.StartNew(async () =>
            {
                Common.ShowToast("正在加载设备列表...");

                var response = await Client.LoadDevices();
                if (response.Result)
                {
                    Console.WriteLine("设备数：" + Client.Devices.Count);
                    Common.UiAction(() => { DeviceListBox.ItemsSource = Client.Devices;DeviceListBox.Items.Refresh(); });

                    Common.ShowToast("设备加载成功");
                }
                else
                {
                    Common.ShowToast("设备加载失败");
                }
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            LoadDevices();
        }
    }
}