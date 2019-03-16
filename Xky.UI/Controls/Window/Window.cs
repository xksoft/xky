using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xky.UI.Data;
using Xky.UI.Tools;
using System.Windows.Shell;
using Xky.UI.Tools.Extension;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;


namespace Xky.UI.Controls
{
    [TemplatePart(Name = ElementNonClientArea, Type = typeof(UIElement))]
    public class Window : System.Windows.Window
    {
        private const string ElementNonClientArea = "PART_NonClientArea";

        public static readonly DependencyProperty NonClientAreaContentProperty = DependencyProperty.Register(
            "NonClientAreaContent", typeof(object), typeof(Window), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty CloseButtonHoverBackgroundProperty = DependencyProperty.Register(
            "CloseButtonHoverBackground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty CloseButtonHoverForegroundProperty =
            DependencyProperty.Register(
                "CloseButtonHoverForeground", typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty CloseButtonBackgroundProperty = DependencyProperty.Register(
            "CloseButtonBackground", typeof(Brush), typeof(Window), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty CloseButtonForegroundProperty = DependencyProperty.Register(
            "CloseButtonForeground", typeof(Brush), typeof(Window),
            new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty OtherButtonBackgroundProperty = DependencyProperty.Register(
            "OtherButtonBackground", typeof(Brush), typeof(Window), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty OtherButtonForegroundProperty = DependencyProperty.Register(
            "OtherButtonForeground", typeof(Brush), typeof(Window),
            new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty OtherButtonHoverBackgroundProperty = DependencyProperty.Register(
            "OtherButtonHoverBackground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty OtherButtonHoverForegroundProperty =
            DependencyProperty.Register(
                "OtherButtonHoverForeground", typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty NonClientAreaBackgroundProperty = DependencyProperty.Register(
            "NonClientAreaBackground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty NonClientAreaForegroundProperty = DependencyProperty.Register(
            "NonClientAreaForeground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty NonClientAreaHeightProperty = DependencyProperty.Register(
            "NonClientAreaHeight", typeof(double), typeof(Window),
            new PropertyMetadata(28.0));

        public static readonly DependencyProperty ShowNonClientAreaProperty = DependencyProperty.Register(
            "ShowNonClientArea", typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.TrueBox, OnShowNonClientAreaChanged));

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle", typeof(bool), typeof(Window), new PropertyMetadata(ValueBoxes.FalseBox));


        private UIElement _nonClientArea;

        private bool _showNonClientArea = true;

        private double _tempNonClientAreaHeight;

        private Thickness _tempThickness;

        public Window()
        {
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                GlassFrameThickness = new Thickness(0, 0, 0, 1)
            };
            BindingOperations.SetBinding(chrome, WindowChrome.CaptionHeightProperty,
                new Binding(NonClientAreaHeightProperty.Name) {Source = this});
            WindowChrome.SetWindowChrome(this, chrome);

            Loaded += delegate
            {
                _tempThickness = BorderThickness;
                if (WindowState == WindowState.Maximized)
                {
                    BorderThickness = new Thickness();
                }

                CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                    (s, e) => WindowState = WindowState.Minimized));
                CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand,
                    (s, e) => WindowState = WindowState.Maximized));
                CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                    (s, e) => WindowState = WindowState.Normal));
                CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (s, e) => Close()));
                CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));

                _tempNonClientAreaHeight = NonClientAreaHeight;


                SwitchShowNonClientArea(_showNonClientArea);
            };
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;


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

        #region 界面UI事件

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            //  DropShadowEffect.Color = Color.FromArgb(204, 200, 200, 200);
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            //   DropShadowEffect.Color = Color.FromArgb(204, 0, 0, 0);
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
                Margin = new Thickness(left, top, right, bottom);
            }
            else
            {
                Margin = new Thickness(10);
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

        public Brush CloseButtonBackground
        {
            get => (Brush) GetValue(CloseButtonBackgroundProperty);
            set => SetValue(CloseButtonBackgroundProperty, value);
        }

        public Brush CloseButtonForeground
        {
            get => (Brush) GetValue(CloseButtonForegroundProperty);
            set => SetValue(CloseButtonForegroundProperty, value);
        }

        public Brush OtherButtonBackground
        {
            get => (Brush) GetValue(OtherButtonBackgroundProperty);
            set => SetValue(OtherButtonBackgroundProperty, value);
        }

        public Brush OtherButtonForeground
        {
            get => (Brush) GetValue(OtherButtonForegroundProperty);
            set => SetValue(OtherButtonForegroundProperty, value);
        }

        public double NonClientAreaHeight
        {
            get => (double) GetValue(NonClientAreaHeightProperty);
            set => SetValue(NonClientAreaHeightProperty, value);
        }


        public object NonClientAreaContent
        {
            get => GetValue(NonClientAreaContentProperty);
            set => SetValue(NonClientAreaContentProperty, value);
        }

        public Brush CloseButtonHoverBackground
        {
            get => (Brush) GetValue(CloseButtonHoverBackgroundProperty);
            set => SetValue(CloseButtonHoverBackgroundProperty, value);
        }

        public Brush CloseButtonHoverForeground
        {
            get => (Brush) GetValue(CloseButtonHoverForegroundProperty);
            set => SetValue(CloseButtonHoverForegroundProperty, value);
        }

        public Brush OtherButtonHoverBackground
        {
            get => (Brush) GetValue(OtherButtonHoverBackgroundProperty);
            set => SetValue(OtherButtonHoverBackgroundProperty, value);
        }

        public Brush OtherButtonHoverForeground
        {
            get => (Brush) GetValue(OtherButtonHoverForegroundProperty);
            set => SetValue(OtherButtonHoverForegroundProperty, value);
        }

        public Brush NonClientAreaBackground
        {
            get => (Brush) GetValue(NonClientAreaBackgroundProperty);
            set => SetValue(NonClientAreaBackgroundProperty, value);
        }

        public Brush NonClientAreaForeground
        {
            get => (Brush) GetValue(NonClientAreaForegroundProperty);
            set => SetValue(NonClientAreaForegroundProperty, value);
        }

        public bool ShowNonClientArea
        {
            get => (bool) GetValue(ShowNonClientAreaProperty);
            set => SetValue(ShowNonClientAreaProperty, value);
        }

        public bool ShowTitle
        {
            get => (bool) GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        private static void OnShowNonClientAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (Window) d;
            ctl.SwitchShowNonClientArea((bool) e.NewValue);
        }

        private void SwitchShowNonClientArea(bool showNonClientArea)
        {
            if (_nonClientArea == null)
            {
                _showNonClientArea = showNonClientArea;
                return;
            }

            if (showNonClientArea)
            {
                _nonClientArea.Show(true);
                NonClientAreaHeight = _tempNonClientAreaHeight;
            }
            else
            {
                _nonClientArea.Show(false);
                _tempNonClientAreaHeight = NonClientAreaHeight;
                NonClientAreaHeight = 0;
            }
        }


        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new Thickness();
            }
            else if (WindowState == WindowState.Normal)
            {
                BorderThickness = _tempThickness;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _nonClientArea = GetTemplateChild(ElementNonClientArea) as UIElement;

            if (SizeToContent != SizeToContent.WidthAndHeight)
                return;

            SizeToContent = SizeToContent.Height;
            Dispatcher.BeginInvoke(new Action(() => { SizeToContent = SizeToContent.WidthAndHeight; }));
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var point = WindowState == WindowState.Maximized
                ? new Point(0, 28)
                : new Point(Left, Top + 28);
            SystemCommands.ShowSystemMenu(this, point);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
        }
    }
}