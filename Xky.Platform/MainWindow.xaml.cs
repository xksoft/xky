using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Model;

using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace Xky.Platform
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MirrorClient _client = new MirrorClient();
        private ObservableCollection<Device> devicelist;

        private Dictionary<string, string> dic = new Dictionary<string, string>();

        private string session = "";

        public MainWindow()
        {
            InitializeComponent();
            var wc = new WindowChrome
            {
                CaptionHeight = 30,
                GlassFrameThickness = new Thickness(0),
                CornerRadius = new CornerRadius(0),
                UseAeroCaptionButtons = false,
                ResizeBorderThickness = new Thickness(15)
            };
            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;
            SizeChanged += MainWindow_OnSizeChanged;
            WindowChrome.SetWindowChrome(this, wc);

            try
            {
                using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    if (Math.Abs(graphics.DpiX - 96) > 0)
                    {
                        TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
                        FontFamily = new FontFamily("Microsoft Yahei");
                    }
                    else
                    {
                        TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
                        FontFamily = new FontFamily("SimSun");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            var obj = _client.Post("login",
                JObject.Parse("{'key':'xky','account':'" + text_account.Text + "','password':'" +
                              text_password.Password + "'}"));
            if (obj.ContainsKey("session"))
            {
                session = obj.GetValue("session").ToString();
                LoadDeviceList();
            }
        }

        public void LoadDeviceList()
        {
            var obj = _client.Get("get_device_list?session=" + session + "&source=true", JObject.Parse("{}"));
            // Console.WriteLine(obj);
            if (obj.ContainsKey("list"))
            {
                devicelist = new ObservableCollection<Device>();
                var deviceobjects = obj.GetValue("list").ToObject<List<JObject>>();
                Console.WriteLine(obj["onlines"] as JArray);
                foreach (var deviceobject in deviceobjects)
                {
                    Console.WriteLine(deviceobject.GetValue("t_sn"));

                    if ((obj["onlines"] as JArray).Values<string>().ToList()
                        .Contains(deviceobject.GetValue("t_sn").ToString()))
                        devicelist.Add(new Device
                        {
                            Sn = deviceobject.GetValue("t_sn").ToString(),
                            Id = Convert.ToInt32(deviceobject.GetValue("t_id").ToString()),
                            Name = deviceobject.GetValue("t_name").ToString()
                        });
                }

                DataContext = devicelist;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var device = listbox.SelectedItem;
            if (device != null)
            {
                _client.Connect(((Device) device).Sn, session);
                Mirror.ShowLoading();
                Mirror.SetClient(_client);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _client.EmitEvent(JObject.Parse("{type: \"device_button\", name: \"code\", key: 3}"));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _client.EmitEvent(JObject.Parse("    { type: \"device_button\", name: \"code\", key: 187}"));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _client.EmitEvent(JObject.Parse("      { type: \"device_button\", name: \"code\", key: 4}"));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
        }

        #region 界面UI事件

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            DropShadowEffect.Color = Color.FromArgb(204, 200, 200, 200);
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            DropShadowEffect.Color = Color.FromArgb(204, 0, 0, 0);
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                var top = (e.NewSize.Height - SystemParameters.PrimaryScreenHeight) / 2 + SystemParameters.WorkArea.Top;
                var left = (e.NewSize.Width - SystemParameters.WorkArea.Width) / 2 + SystemParameters.WorkArea.Left;
                var right = (e.NewSize.Width - SystemParameters.WorkArea.Width) / 2 +
                            SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right;
                var bottom = (e.NewSize.Height - SystemParameters.PrimaryScreenHeight) / 2 +
                             SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom;
                MainGrid.Margin = new Thickness(left, top, right, bottom);
                BtnRestore.Visibility = Visibility.Visible;
                BtnMax.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainGrid.Margin = new Thickness(10);
                BtnRestore.Visibility = Visibility.Collapsed;
                BtnMax.Visibility = Visibility.Visible;
            }
        }

        private void Btn_close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_max(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
        }


        private void Btn_min(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion
    }
}