using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Xky.Core.Common;
using Xky.Core.Model;
using Xky.Socket.Client;

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


        /// <summary>
        /// 屏幕显示类
        /// </summary>
        public MirrorScreen()
        {
            InitializeComponent();
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
                lock ("connect")
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
                        if (_bindingSource < 20)
                        {
                            _bindingSource++;
                            if (_bindingSource == 20) CurrentDevice.ScreenShot = _writeableBitmap;
                        }


                        _writeableBitmap?.WritePixels(new Int32Rect(0, 0, width, height), intprt, width * height * 4,
                            stride);

                        if (IsShowFps) _averageNumber.Push(1);

                        if (!_isShow)
                        {
                            AddLabel("成功解析画面..", Colors.Lime);
                            _isShow = true;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region  Loading和日志

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public void AddLabel(string msg, Color color)
        {
            OnShowLog?.Invoke(this, msg, color);
        }

        #endregion


        #region 屏幕连接

        private H264Decoder _decoder;

        private Socket.Client.Socket _socket;

        /// <summary>
        /// 当前连接的设备模型
        /// </summary>
        public static Device CurrentDevice;

        private int _bindingSource;

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="model"></param>
        public void Connect(Device model)
        {
            if (_decoder == null)
            {
                _decoder = new H264Decoder();
                _decoder.OnDecodeBitmapSource += Decoder_OnDecodeBitmapSource;
            }

            if (CurrentDevice != null && model.Sn != CurrentDevice.Sn)
                Dispatcher.Invoke(() =>
                {
                    if (CurrentDevice.ScreenShot != null)
                        CurrentDevice.ScreenShot = CurrentDevice.ScreenShot.Clone();
                });

            AddLabel("正在连接设备[" + model.Name + "]..", Colors.GreenYellow);
            CurrentDevice = Client.GetDevice(model.Sn);


            if (CurrentDevice == null) throw new Exception("无法获取这个设备的信息");

            if (Client.LocalNodes.ContainsKey(CurrentDevice.NodeSerial))
            {
                CurrentDevice.NodeUrl = "http://" + Client.LocalNodes[CurrentDevice.NodeSerial].Ip + ":8080";
                Console.WriteLine(CurrentDevice.NodeUrl);
            }


            if (CurrentDevice.NodeUrl == "") throw new Exception("该设备没有设置P2P转发模式");


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
                    {"sn", CurrentDevice.Sn},
                    {"action", "mirror"},
                    {"v2", "true"},
                    {"hash", CurrentDevice.ConnectionHash}
                },
                Path = "/xky",
                Transports = ImmutableList.Create("websocket")
            };
            _socket = IO.Socket(CurrentDevice.NodeUrl, options);
            _socket.On(Socket.Client.Socket.EventConnect, () => { Console.WriteLine("Connected"); });
            _socket.On(Socket.Client.Socket.EventDisconnect, () => { Console.WriteLine("Disconnected"); });
            _socket.On(Socket.Client.Socket.EventError, () => { Console.WriteLine("ERROR"); });
            _socket.On("event", json => { Console.WriteLine(json); });
            _socket.On("h264", data => { _decoder?.Decode((byte[]) data); });
        }


        /// <summary>
        /// 给设备发送事件
        /// </summary>
        /// <param name="jObject"></param>
        public void EmitEvent(JObject jObject)
        {
            _socket?.Emit("event", jObject);
        }


        /// <summary>
        /// 显示日志
        /// </summary>
        public event ShowLog OnShowLog;

        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="log"></param>
        /// <param name="color"></param>
        public delegate void ShowLog(object sender, string log, Color color);

        #endregion

        #region 属性

        /// <summary>
        /// 是否显示fps
        /// </summary>
        public bool IsShowFps
        {
            get => (bool) GetValue(IsShowFpsProperty);
            set => SetValue(IsShowFpsProperty, value);
        }

        /// <summary>
        /// 是否显示fps属性
        /// </summary>
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


        /// <summary>
        /// 是否显示箭头
        /// </summary>
        public bool IsShowArrow
        {
            get => (bool) GetValue(IsShowArrowProperty);
            set => SetValue(IsShowArrowProperty, value);
        }

        /// <summary>
        /// 是否显示箭头属性
        /// </summary>
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

        private void ScreenImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var postion = e.GetPosition(ScreenImage);
            var json = new JObject
            {
                {"type", "wheel"},
                {"x", (postion.X / RenderSize.Width).ToString("F4")},
                {"y", (postion.Y / RenderSize.Height).ToString("F4")},
                {"dx", e.Delta / 100},
                {"dy", e.Delta / 100}
            };
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
    }
}