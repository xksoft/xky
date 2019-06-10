using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
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
                Common.UiAction(() => { LoadingTextBlock.Text = "正在检查系统更新..."; });
#if publish
                //正式版则检查更新
                Upgrade();
#endif
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

        private void Upgrade()
        {
            try
            {
                if (File.Exists("noupgrade.txt"))
                    return;
                var updateJson = HttpHelper.Get("https://static.xky.com/upgrade/x/update.json?tick="+DateTime.Now.Millisecond);
                var jarray = JsonConvert.DeserializeObject<JArray>(updateJson);
                var needUpgrade = new JArray();
                foreach (var t in jarray)
                {
                    var json = (JObject) t;
                    if (File.Exists(System.IO.Path.Combine(".\\", json["file"].ToString()) + ".noupgrade"))
                        continue;
                    if (json["md5"].ToString() !=
                        FileHelper.GetFileHash(System.IO.Path.Combine(".\\", json["file"].ToString())))
                    {
                        needUpgrade.Add(json);
                    }
                }
                Console.WriteLine(needUpgrade);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private  void DownFile(string filename, string softkey, string path, string msg)
        {
//            try
//            {
//                path = path.TrimStart('\\');
//                var url = "http://upgrade.xksoft.com/" + softkey + "/" + path.Replace("\\", "/") + filename + "?tick=" +
//                          DateTime.Now.Ticks;
//                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
//                httpWebRequest.Proxy = null;
//
//                httpWebRequest.ContentType = "text/xml";
//                httpWebRequest.Method = "GET";
//                httpWebRequest.Timeout = 60 * 1000;
//                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
//                var sm = httpWebResponse.GetResponseStream();
//                var l = httpWebResponse.ContentLength;
//                var msTemp = new MemoryStream();
//                int len;
//                var buff = new byte[4096];
//                while ((len = sm.Read(buff, 0, 4096)) > 0)
//                {
//                    msTemp.Write(buff, 0, len);
//
//                    Loading.UpdateState(msg.Replace("$1",
//                        (msTemp.Length / (decimal)1024).ToString("F2") + "KB/" + (l / (decimal)1024).ToString("F2") +
//                        "KB"));
//                    _downLength += len;
//                    Loading.Progress((double)_downLength / _allLength);
//                }
//
//                if (path != "")
//                {
//                    if (!Directory.Exists(path))
//                        Directory.CreateDirectory(path);
//                }
//
//                if (filename.EndsWith(".exe") || filename.EndsWith(".dll"))
//                {
//                    if (File.Exists(Path.GetFullPath(FolderPath + path + filename)))
//                        File.Move(Path.GetFullPath(FolderPath + path + filename),
//                            Path.GetFullPath(FolderPath + path + filename + DateTime.Now.Ticks) + ".old");
//                }
//
//                File.WriteAllBytes(Path.GetFullPath(FolderPath + path + filename), msTemp.ToArray());
//            }
//            catch (Exception ex)
//            {
//                //MessageBox.Show("抱歉，下载更新程序时出错：" + ex.Message + " " + ex.StackTrace + " " + ex.Source, "引导出错",
//                //    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
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
            Common.OpenUrl("https://www.xky.com/user");
        }
    }
}