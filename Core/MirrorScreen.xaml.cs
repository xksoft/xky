using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using Xky.Core.Common;
using Xky.Core.Model;
using Xky.Socket.Client;
using Timer = System.Timers.Timer;

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
            if (Decoder != null)
                Decoder.OnDecodeBitmapSource += Decoder_OnDecodeBitmapSource;
            AddLabel("Mirror Initialized", Colors.Lime);
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
                        if (ScreenImage.Source == null || _writeableBitmap.PixelWidth != width)
                        {
                            _writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
                            ScreenImage.Source = _writeableBitmap;
                            CurrentDevice.ScreenShot = _writeableBitmap;
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
                            ScreenImage.Visibility = Visibility.Visible;
                            AddLabel("成功解析画面...", Colors.Lime);
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

        private bool _autoremoveLabel = false;

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        /// <param name="autoremove"></param>
        public void AddLabel(string msg, Color color, bool autoremove = true)
        {
            //OnShowLog?.Invoke(this, msg, color);
            _autoremoveLabel = autoremove;
            Client.StartAction(() =>
            {
                Client.MainWindow.Dispatcher.Invoke(() =>
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    StatusLabel.Opacity = 1;
                    StatusLabel.Foreground = new SolidColorBrush(color);
                    StatusLabel.Content = msg;
                });

                if (_autoremoveLabel)
                {
                    var count = 1500;
                    while (count > 0)
                    {
                        count--;
                        Thread.Sleep(1);
                        if (!_autoremoveLabel)
                        {
                            Client.MainWindow.Dispatcher.Invoke(() => { StatusLabel.Opacity = 1; });
                            return;
                        }
                    }

                    for (var i = 0; i < 50; i++)
                    {
                        if (!_autoremoveLabel)
                        {
                            Client.MainWindow.Dispatcher.Invoke(() => { StatusLabel.Opacity = 1; });
                            return;
                        }

                        Client.MainWindow.Dispatcher.Invoke(() => { StatusLabel.Opacity -= 0.02; });
                        Thread.Sleep(20);
                    }

                    Client.MainWindow.Dispatcher.Invoke(() => { StatusLabel.Visibility = Visibility.Collapsed; });
                }
            });
        }

        #endregion


        #region 屏幕连接

        /// <summary>
        /// 视频数据解码器
        /// </summary>
        public static H264Decoder Decoder = null;

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
            if (CurrentDevice != null && model.Sn != CurrentDevice.Sn)
                Dispatcher.Invoke(() =>
                {
                    if (CurrentDevice.ScreenShot != null)
                        CurrentDevice.ScreenShot = CurrentDevice.ScreenShot.Clone();
                });

            AddLabel("正在连接设备[" + model.Name + "]..", Colors.GreenYellow, false);
            CurrentDevice = Client.GetDevice(model.Sn);


            if (CurrentDevice == null) throw new Exception("无法获取这个设备的信息");
            var node = Client.Nodes.ToList().Find(p => p.Serial == CurrentDevice.NodeSerial);
            if (node != null)
            {
                CurrentDevice.NodeUrl = node.NodeUrl;
                Console.WriteLine("连接屏幕：" + CurrentDevice.NodeUrl);
            }

            if (CurrentDevice.NodeUrl == "")
            {
                AddLabel("设备所属节点服务器尚未连接，请稍后...", Colors.OrangeRed, false);
                throw new Exception("该设备没有设置P2P转发模式且尚未在局域网中发现节点服务器");
            }


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
            _socket.On(Socket.Client.Socket.EventDisconnect, () =>
            {
                if (CurrentDevice.Sn == model.Sn)
                {
                    Dispatcher.Invoke(() => { ScreenImage.Visibility = Visibility.Collapsed; });
                    AddLabel(model.Name + " 已断开连接...", Colors.OrangeRed, false);
                }
            });
            _socket.On(Socket.Client.Socket.EventError, () => { Console.WriteLine("ERROR"); });
            _socket.On("event", json =>
            {
                //Console.WriteLine(json);
            });
            _socket.On("h264", data => { Decoder?.Decode((byte[]) data); });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            _socket?.Disconnect();
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
                if (Client.BatchControl)
                {
                    Client.CallBatchControlEnvent(new JObject
                    {
                        {"type", "device_button"},
                        {"name", "back"}
                    });
                }
                else
                {
                    EmitEvent(
                        new JObject
                        {
                            {"type", "device_button"},
                            {"name", "back"}
                        });
                }
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
                if (Client.BatchControl)
                {
                    Client.CallBatchControlEnvent(json);
                }
                else
                {
                    EmitEvent(json);
                }

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
                if (Client.BatchControl)
                {
                    Client.CallBatchControlEnvent(json);
                }
                else
                {
                    EmitEvent(json);
                }

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
            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(json);
            }
            else
            {
                EmitEvent(json);
            }

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
            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(json);
            }
            else
            {
                EmitEvent(json);
            }
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
            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(json);
            }
            else
            {
                EmitEvent(json);
            }
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
            var jObject = new JObject
            {
                {"text", e.Text},
                {"type", "input"}
            };
            if (Client.BatchControl)
            {
                Client.CallBatchControlEnvent(jObject);
            }
            else
            {
                EmitEvent(jObject);
            }
        }

        private void MyInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                {
                    var jObject = new JObject
                    {
                        {"key", 67},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }

                case Key.Return:
                {
                    var jObject = new JObject
                    {
                        {"key", 66},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }

                case Key.Space:
                {
                    var jObject = new JObject
                    {
                        {"key", 62},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }

                case Key.Up:
                {
                    var jObject = new JObject
                    {
                        {"key", 19},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }

                case Key.Down:
                {
                    var jObject = new JObject
                    {
                        {"key", 20},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }

                case Key.Left:
                {
                    var jObject = new JObject
                    {
                        {"key", 21},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }

                case Key.Right:
                {
                    var jObject = new JObject
                    {
                        {"key", 22},
                        {"name", "code"},
                        {"type", "device_button"}
                    };
                    if (Client.BatchControl)
                    {
                        Client.CallBatchControlEnvent(jObject);
                    }
                    else
                    {
                        EmitEvent(jObject);
                    }

                    return;
                }
            }

            if (e.Key != Key.V || (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control) return;
            var text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                var jObject = new JObject
                {
                    {"text", text},
                    {"type", "input"}
                };
                if (Client.BatchControl)
                {
                    Client.CallBatchControlEnvent(jObject);
                }
                else
                {
                    EmitEvent(jObject);
                }
            }
        }

        #endregion
    }
}