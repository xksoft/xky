using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private void MyMirrorScreen_OnChangeSource(ImageSource source)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                //克隆一份最后的图像给上个设备的ScreenShot，然后脱离引用，避免被其他device屏幕覆盖
                if (_lastDevice != null && _lastDevice.Sn != device.Sn)
                {
                    _lastDevice.ScreenShot = _lastDevice.ScreenShot.Clone();
                }

                device.ScreenShot = source;
                _lastDevice = device;
            }
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
                    Common.UiAction(() => { DeviceListBox.ItemsSource = Client.Devices; });

                    Common.ShowToast("设备加载成功");
                }
                else
                {
                    Common.ShowToast("设备加载失败");
                }
            });
        }


        private Device _lastDevice;

        private void DeviceListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is Device device)
            {
                MyMirrorScreen.Connect(device);
            }
        }
    }
}