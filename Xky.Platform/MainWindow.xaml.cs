using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;
using Xky.Core;
using Xky.Platform.UserControl;
using Xky.Platform.UserControl.Pages;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace Xky.Platform
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var wc = new WindowChrome
            {
                CaptionHeight = 30,
                GlassFrameThickness = new Thickness(0),
                CornerRadius = new CornerRadius(0),
                UseAeroCaptionButtons = false,
                ResizeBorderThickness = new Thickness(25)
            };
            WindowChrome.SetWindowChrome(this, wc);
            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;
            SizeChanged += MainWindow_OnSizeChanged;


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

            _buttonStatus.Baseurl = "Resources/Icon/ControlBox/drak/";
            DataContext = _buttonStatus;

            //初始化页面加载、方便调用UI线程委托
            Common.MainWindow = this;
            Client.MainWindow = this;

            //加载login页面
            LoginTabItem.ClickDown(null, null);

            //启动状态定时器
            new Timer {Interval = 1000, Enabled = true}.Elapsed += MainWindow_Elapsed;
        }

        private void MainWindow_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.UiAction(() =>
            {
                CoreStatus.Fill = Client.CoreConnected
                    ? (SolidColorBrush) FindResource("OnLine")
                    : (SolidColorBrush) FindResource("OffLine");

                //速率单位换算
                var bitcount = Client.BitAverageNumber.GetAverageNumber() * 8;
                string bitspeed;
                if (bitcount > 1024 * 1024)
                    bitspeed = (bitcount / (decimal) 1024 / 1024).ToString("F2") + " Mbps";
                else if (bitcount > 1024)
                    bitspeed = (bitcount / (decimal) 1024).ToString("F2") + " Kbps";
                else
                    bitspeed = bitcount + " bps";

                StatusText.Text = "速率：" + bitspeed + " 节点：" + Client.Nodes.Count + " 设备：" + Client.Devices.Count +
                                  " 线程：" + Client.Threads;
            });
        }

        #region 事件

        private void MyTabItem_OnOnClickEvent(MyTabItem sender, string pagename, bool dark)
        {
            _buttonStatus.Baseurl = dark ? "Resources/Icon/ControlBox/dark/" : "Resources/Icon/ControlBox/default/";
            if (pagename == null)
                return;
            if (_userControlDic.ContainsKey(pagename))
                MainContent.Content = _userControlDic[pagename];
            else
                switch (pagename)
                {
                    case "Login":
                    {
                        var page = new MyLogin();
                        _userControlDic.Add(pagename, page);

                        MainContent.Content = page;
                        break;
                    }
                    case "MainControl":
                    {
                        var page = new MyMainControl();
                        _userControlDic.Add(pagename, page);
                        MainContent.Content = page;
                        break;
                    }
                    case "Task":
                    {
                        var page = new MyTask();
                        _userControlDic.Add(pagename, page);
                        MainContent.Content = page;
                        break;
                    }
                    case "Setting":
                    {
                        var page = new MySetting();
                        _userControlDic.Add(pagename, page);
                        MainContent.Content = page;
                        break;
                    }
                }
        }

        #endregion

        #region 基础属性

        //缓存内部页面提高加载效率
        private readonly Dictionary<string, System.Windows.Controls.UserControl> _userControlDic =
            new Dictionary<string, System.Windows.Controls.UserControl>();

        //记录窗口按钮状态
        private readonly ButtonStatusModel _buttonStatus = new ButtonStatusModel();

        #endregion

        #region 界面UI事件和属性

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
                var wc = new WindowChrome
                {
                    CaptionHeight = 30,
                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0),
                    UseAeroCaptionButtons = false,
                    ResizeBorderThickness = new Thickness(0)
                };
                WindowChrome.SetWindowChrome(this, wc);
            }
            else
            {
                MainGrid.Margin = new Thickness(20);
                BtnRestore.Visibility = Visibility.Collapsed;
                BtnMax.Visibility = Visibility.Visible;
                var wc = new WindowChrome
                {
                    CaptionHeight = 30,
                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0),
                    UseAeroCaptionButtons = false,
                    ResizeBorderThickness = new Thickness(25)
                };
                WindowChrome.SetWindowChrome(this, wc);
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

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var msg = new MyMessageBox(MessageBoxButton.YesNo) {Height = 100, MessageText = "您确认要关闭系统吗？"};
            Common.ShowMessageControl(msg);

            if (msg.Result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }

    #region 窗体按钮状态转换模型

    public class ButtonStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + "" + parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ButtonStatusModel : INotifyPropertyChanged
    {
        private string _baseurl;

        public string Baseurl
        {
            get => _baseurl;
            set
            {
                _baseurl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Baseurl"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    #endregion
}