using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Common;
using Path = System.IO.Path;

namespace Xky.Platform.Pages
{
    /// <summary>
    ///     MyLogin.xaml 的交互逻辑
    /// </summary>
    public partial class Login
    {
        private long _allLength;
        private long _downLength;

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
                //等一秒再删，不然上个进程没结束，删不了
                Thread.Sleep(1000);
                var oldfiles = FileHelper.GetFileList(".\\", "*.old", true);
                foreach (var oldfile in oldfiles)
                {
                    File.Delete(oldfile);
                }

                var updateJson =
                    HttpHelper.Get("https://static.xky.com/upgrade/x/update.json?tick=" + DateTime.Now.Millisecond);
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
                        _allLength += long.Parse(json["length"].ToString());
                        needUpgrade.Add(json);
                    }
                }

                if (needUpgrade.Count == 0)
                    return;
                Common.UiAction(() => { LoadingBar.Visibility = Visibility.Visible; });
                for (var index = 0; index < needUpgrade.Count; index++)
                {
                    var json = needUpgrade[index];
                    DownFile(json["file"].ToString(),
                        "正在更新 " + json["file"] + " (" + (index + 1) + "/" + needUpgrade.Count + ") $1");
                }

                Common.UiAction(() => { LoadingTextBlock.Text = "全部更新完毕，正在重启本系统以完成更新！"; });
                Thread.Sleep(1000);
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = "xky.platform.exe",
                        UseShellExecute = false
                    }
                };
                p.Start();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void DownFile(string filename, string msg)
        {
            try
            {
                var url = "https://static.xky.com/upgrade/x/" + filename + "?tick=" +
                          DateTime.Now.Ticks;
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
                httpWebRequest.Proxy = null;

                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 60 * 1000;
                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var sm = httpWebResponse.GetResponseStream();
                var l = httpWebResponse.ContentLength;
                var msTemp = new MemoryStream();
                int len;
                var buff = new byte[4096];
                while ((len = sm.Read(buff, 0, 4096)) > 0)
                {
                    msTemp.Write(buff, 0, len);
                    _downLength += len;


                    Common.UiAction(() =>
                    {
                        LoadingTextBlock.Text = msg.Replace("$1",
                            (msTemp.Length / (decimal) 1024).ToString("F2") + "KB/" +
                            (l / (decimal) 1024).ToString("F2") +
                            "KB");
                        LoadingBar.Value = (double) _downLength / _allLength;
                    });
                }

                var fileInfo = new FileInfo(Path.Combine(".\\", filename));


                if (!Directory.Exists(fileInfo.DirectoryName))
                    Directory.CreateDirectory(fileInfo.DirectoryName ?? throw new InvalidOperationException());


                if (filename.EndsWith(".exe") || filename.EndsWith(".dll"))
                {
                    if (fileInfo.Exists)
                        File.Move(fileInfo.FullName,
                            fileInfo.FullName + ".old");
                }


                File.WriteAllBytes(fileInfo.FullName, msTemp.ToArray());
            }
            catch (Exception ex)
            {
                //MessageBox.Show("抱歉，下载更新程序时出错：" + ex.Message + " " + ex.StackTrace + " " + ex.Source, "引导出错",
                //    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                        while (true)
                        {
                            if (Client.CoreConnected)
                            {   Common.MainWindow.MainControlTabItem.ClickDown(null, null);
                                Common.MyMainControl.LoadNodes();
                                Common.MyMainControl.LoadDevices();
                                Common.MyMainControl.LoadModules();
                             
                                break;
                            }
                            else { Thread.Sleep(500); }

                        }
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