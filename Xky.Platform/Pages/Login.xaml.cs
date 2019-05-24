using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Common;
using Xky.Core.UserControl;

namespace Xky.Platform.Pages
{
    /// <summary>
    ///     MyLogin.xaml 的交互逻辑
    /// </summary>
    public partial class Login
    {
        public Login()
        {
            InitializeComponent();
            BtnLogin.Click += Login_Click;
            Client.StartAction(() =>
            {
                var json = Common.LoadJson("license");
                if (json != null) Common.UiAction(() => { LicenseKey.Text = json["license"].ToString(); });
                Common.UiAction(() =>
                {
                    LoadingTextBlock.Text = "正在检查系统更新...";
                });
                MirrorScreen.Decoder = new H264Decoder();
                Thread.Sleep(500);
                Common.UiAction(() =>
                {

                    LoadingTextBlock.Text = "正在预热系统组件...";
                    GridLoading.Visibility = Visibility.Collapsed;
                    GridLogin.Visibility = Visibility.Visible;
                });
            });
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var licensekey = LicenseKey.Text.Trim();

            Client.StartAction(() =>
            {
                Common.UiAction(() =>
                {
                    BtnLogin.Text = "正在授权";
                    BtnLogin.IsEnabled = false;
                });

                var response = Client.AuthLicense(licensekey);
                if (response.Result)
                {
                    Common.SaveJson("license", new JObject {["license"] = licensekey});
                    Common.ShowToast("授权成功:" + Client.License.LicenseName, Colors.Lime, "on");
                    Thread.Sleep(500);

                    Common.UiAction(() =>
                    {
                        GridLogin.Visibility = Visibility.Collapsed;
                        GridInfo.Visibility = Visibility.Visible;
                        Image_Avatar.Source = new BitmapImage(new Uri(
                            "https://static.xky.com/avatar/" + Client.License.Avatra, UriKind.RelativeOrAbsolute));
                        Label_Email.Content = Client.License.Email;
                        Label_Nickname.Content = Client.License.NickName;
                        Label_Phone.Content = Client.License.Phone;
                        Label_License_Name.Content = Client.License.LicenseName;
                        Label_License_Key.Content = Client.License.LicenseKey;
                        Label_License_Expiration_Time.Content =
                            Client.License.LicenseExpiration.ToString("yyyy-MM-dd HH:mm:ss");
                        Common.MainWindow.MainControlTabItem.ClickDown(null, null);
                        Common.MyMainControl.LoadNodes();
                        Common.MyMainControl.LoadDevices();
                        Common.MyMainControl.LoadModules();
                    });
                }
                else
                {
                    Common.ShowToast(response.Message, Color.FromRgb(255, 28, 28));
                }

                Common.UiAction(() =>
                {
                    BtnLogin.Text = "授权";
                    BtnLogin.IsEnabled = true;
                });
            });
        }


        private void Button_EditInfo_Click(object sender, RoutedEventArgs e)
        {
            Common.OpenUrl("https://www.xky.com/user-setting");
        }
    }
}