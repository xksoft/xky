using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using Xky.Core.Common;
using Xky.Core.Model;

namespace Xky.Core
{
    /// <summary>
    ///     MirrorScreen.xaml 的交互逻辑
    /// </summary>
    public partial class MirrorScreen
    {
        private readonly AverageNumber _averageNumber = new AverageNumber(3);
        private readonly Timer _fpsTimer;
        private bool _isShow;
        private WriteableBitmap _writeableBitmap;


        public MirrorScreen()
        {
            InitializeComponent();
            //启动局域网节点探查器
            Client.SearchLocalNode();
            RenderOptions.SetBitmapScalingMode(ScreenImage, BitmapScalingMode.LowQuality);
            _fpsTimer = new Timer {Enabled = true, Interval = 1000};
            _fpsTimer.Elapsed += FpsTimer_Elapsed;
        }

        private void FpsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                {
                    Dispatcher.Invoke(() => { FpsLabel.Content = "FPS:" + _averageNumber.GetAverageNumber(); });
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void Decoder_OnDecodeBitmapSource(object sender, int width, int height, int stride, IntPtr intprt)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    //第一次初始化
                    if (ScreenImage.Source == null)
                    {
                        _writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
                        ScreenImage.Source = _writeableBitmap;
                    }


                    //5帧过后再绑定图像源
                    if (_bindingSource < 5)
                    {
                        _bindingSource++;
                        if (_bindingSource == 5) _device.ScreenShot = _writeableBitmap;
                    }


                    _writeableBitmap?.WritePixels(new Int32Rect(0, 0, width, height), intprt, width * height * 4,
                        stride);

                    if (IsShowFps) _averageNumber.Push(1);

                    if (!_isShow)
                    {
                        AddLabel("成功解析画面..", Colors.Lime);
                        _isShow = true;
                        HideLoading();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #region 屏幕连接

        private H264Decoder _decoder;

        private Socket _socket;

        private Device _device;
        private int _bindingSource;


        public async void Connect(Device model)
        {
            if (_decoder == null)
            {
                _decoder = new H264Decoder();
                _decoder.OnDecodeBitmapSource += Decoder_OnDecodeBitmapSource;
            }

            if (_device != null && model.Sn != _device.Sn)
                Dispatcher.Invoke(() => { _device.ScreenShot = _device.ScreenShot.Clone(); });


            AddLabel("正在获取设备" + model.Sn + "的连接信息..", Colors.White);
            _device = await Client.GetDevice(model.Sn);


            if (_device == null) throw new Exception("无法获取这个设备的信息");

            if (_device.NodeUrl == null) throw new Exception("该设备没有设置P2P转发模式");


            _socket?.Disconnect();
            _socket?.Close();

            //重置
            _bindingSource = 0;
            _isShow = false;

            var options = new IO.Options
            {
                IgnoreServerCertificateValidation = true,
                AutoConnect = true,
                ForceNew = true,
                Query = new Dictionary<string, string>
                {
                    {"sn", _device.Sn},
                    {"action", "mirror"},
                    {"v2", "true"},
                    {"hash", _device.ConnectionHash}
                },
                Path = "/xky",
                Transports = ImmutableList.Create("websocket")
            };
            AddLabel("正在连接..", Colors.White);
            _socket = IO.Socket(_device.NodeUrl, options);
            _socket.On(Socket.EVENT_CONNECT, () => { Console.WriteLine("Connected"); });
            _socket.On(Socket.EVENT_DISCONNECT, () => { Console.WriteLine("Disconnected"); });
            _socket.On(Socket.EVENT_ERROR, () => { Console.WriteLine("ERROR"); });
            _socket.On("event", json => { Console.WriteLine(json); });
            _socket.On("h264", data => { _decoder?.Decode((byte[]) data); });
        }


        public void EmitEvent(JObject jObject)
        {
            _socket?.Emit("event", jObject);
        }

        #endregion

        #region 属性

        public bool IsShowFps
        {
            get => (bool) GetValue(IsShowFpsProperty);
            set => SetValue(IsShowFpsProperty, value);
        }

        public static readonly DependencyProperty IsShowFpsProperty =
            DependencyProperty.Register("IsShowFps", typeof(bool), typeof(MirrorScreen), new PropertyMetadata(true,
                (o, e) =>
                {
                    var li = (MirrorScreen) o;


                    if ((bool) e.NewValue == false)
                    {
                        li._fpsTimer.Enabled = false;
                        li.FpsLabel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        li._fpsTimer.Enabled = true;
                        li.FpsLabel.Visibility = Visibility.Visible;
                    }
                }));

        public bool IsShowLog
        {
            get => (bool) GetValue(IsShowLogProperty);
            set => SetValue(IsShowLogProperty, value);
        }

        public static readonly DependencyProperty IsShowLogProperty =
            DependencyProperty.Register("IsShowLog", typeof(bool), typeof(MirrorScreen), new PropertyMetadata(true,
                (o, e) =>
                {
                    var li = (MirrorScreen) o;

                    li.LogPanel.Visibility = (bool) e.NewValue == false ? Visibility.Collapsed : Visibility.Visible;
                }));

        public bool IsShowArrow
        {
            get => (bool) GetValue(IsShowArrowProperty);
            set => SetValue(IsShowArrowProperty, value);
        }

        public static readonly DependencyProperty IsShowArrowProperty =
            DependencyProperty.Register("IsShowArrow", typeof(bool), typeof(MirrorScreen), new PropertyMetadata(true,
                (o, e) => { }));

        #endregion

        #region 屏幕操作

        private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var postion = e.GetPosition(ScreenImage);
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (IsShowArrow) Tap.Fill = new SolidColorBrush(Color.FromArgb(126, 255, 182, 0));

                EmitEvent(
                    new JObject
                    {
                        {"type", "device_button"},
                        {"name", "back"}
                    });
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsShowArrow) Tap.Fill = new SolidColorBrush(Color.FromArgb(126, 0, 255, 0));

                var json = new JObject
                {
                    {"type", "mousedown"},
                    {"x", (postion.X / RenderSize.Width).ToString("F4")},
                    {"y", (postion.Y / RenderSize.Height).ToString("F4")}
                };
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    json.Add("zoom", true);
                EmitEvent(json);
                MyInput.Focus();
            }

            if (IsShowArrow)
            {
                Tap.Visibility = Visibility.Visible;
                Tap.SetValue(Window.TopProperty, postion.Y - 15);
                Tap.SetValue(Window.LeftProperty, postion.X - 15);
            }
        }

        private void Image_OnMouseMove(object sender, MouseEventArgs e)
        {
            var postion = e.GetPosition(ScreenImage);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var json = new JObject
                {
                    {"type", "mousedrag"},
                    {"x", (postion.X / RenderSize.Width).ToString("F4")},
                    {"y", (postion.Y / RenderSize.Height).ToString("F4")}
                };
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    json.Add("zoom", true);
                EmitEvent(json);
                if (IsShowArrow)
                {
                    Tap.SetValue(Window.TopProperty, postion.Y - 15);
                    Tap.SetValue(Window.LeftProperty, postion.X - 15);
                }
            }
        }

        private void Image_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var postion = e.GetPosition(ScreenImage);
            var json = new JObject
            {
                {"type", "mouseup"},
                {"x", (postion.X / RenderSize.Width).ToString("F4")},
                {"y", (postion.Y / RenderSize.Height).ToString("F4")}
            };
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                json.Add("zoom", true);
            EmitEvent(json);
            if (IsShowArrow) Tap.Visibility = Visibility.Collapsed;
        }


        private void Image_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var postion = e.GetPosition(ScreenImage);
            var json = new JObject
            {
                {"type", "mouseup"},
                {"x", (postion.X / RenderSize.Width).ToString("F4")},
                {"y", (postion.Y / RenderSize.Height).ToString("F4")}
            };
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                json.Add("zoom", true);
            EmitEvent(json);
        }

        private void MyInput_LostFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("丢失焦点");
        }

        private void MyInput_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("获取焦点");
        }

        private void MyInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            EmitEvent(new JObject
            {
                {"text", e.Text},
                {"type", "input"}
            });
        }

        private void MyInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 67},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
                case Key.Return:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 66},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
                case Key.Space:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 62},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
                case Key.Up:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 19},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
                case Key.Down:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 20},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
                case Key.Left:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 21},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
                case Key.Right:
                {
                    EmitEvent(new JObject
                    {
                        {"key", 22},
                        {"name", "code"},
                        {"type", "device_button"}
                    });
                    return;
                }
            }

            if (e.Key != Key.V || (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control) return;
            var text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
                EmitEvent(new JObject
                {
                    {"text", text},
                    {"type", "input"}
                });
        }

        #endregion

        #region  Loading和日志

        private void HideLoading()
        {
            var myBrush = new SolidColorBrush();
            var myColorAnimation = new ColorAnimation
            {
                From = Colors.White,
                To = Colors.Transparent,
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                AutoReverse = false
            };
            myColorAnimation.Completed += MyColorAnimation_Completed;

            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation, HandoffBehavior.Compose);

            ScreenLoading.Foreground = myBrush;
        }

        public void ShowLoading()
        {
            //  ScreenLoading.Visibility = Visibility.Visible;
        }


        private void MyColorAnimation_Completed(object sender, EventArgs e)
        {
            // ScreenLoading.Visibility = Visibility.Collapsed;
        }


        private void HideLabel(Label label)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(3 * 1000));
                Dispatcher.Invoke(() =>
                {
                    var myBrush = new SolidColorBrush();
                    ScreenLoading.IsActive = false;
                    var myColorAnimation = new ColorAnimation
                    {
                        From = ((SolidColorBrush) label.Foreground).Color,
                        To = Colors.Transparent,
                        Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                        AutoReverse = false
                    };
                    myColorAnimation.Completed += delegate { LogPanel.Children.Remove(label); };
                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation, HandoffBehavior.Compose);
                    label.Foreground = myBrush;
                });
            });
        }

        public void AddLabel(string msg, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsShowLog)
                {
                    var label = new Label
                    {
                        Content = msg,
                        Effect = new DropShadowEffect
                        {
                            Color = Colors.Black,
                            Direction = 300,
                            ShadowDepth = 1,
                            BlurRadius = 0,
                            Opacity = 1
                        },
                        FontSize = 14,
                        Foreground = new SolidColorBrush(color),
                        Style = null
                    };
                    LogPanel.Children.Add(label);
                    HideLabel(label);
                }
            });
        }

        #endregion
    }
}