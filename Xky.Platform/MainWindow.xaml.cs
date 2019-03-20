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
using Xky.Platform.UserControl;
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

            MainContent.Content = new MyLogin();
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
                MainGrid.Margin = new Thickness(20);
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