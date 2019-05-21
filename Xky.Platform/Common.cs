using System;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Platform.Properties;
using Xky.Platform.Pages;
using System.Diagnostics;
using Xky.Core.UserControl;

namespace Xky.Platform
{
    public static class Common
    {
        public static MainWindow MainWindow;
        public static MainControl MyMainControl;

        public static void OpenUrl(string url)
        {
            try { Process.Start("explorer.exe", url); } catch {
                var msg = new MyMessageBox(MessageBoxButton.YesNo) { MessageText = "无法调用系统浏览器，请手动打开浏览器并输入："+url };
                Common.ShowMessageControl(msg);
            }

        }
        public static void PlaySound(string name)
        {
            switch (name)
            {
                case "on":
                {
                    var player = new SoundPlayer(Resources.voice_on);
                    player.Play();
                    break;
                }
                case "off":
                {
                    var player = new SoundPlayer(Resources.voice_off);
                    player.Play();
                    break;
                }
                case "alert":
                {
                    var player = new SoundPlayer(Resources.voice_alert);
                    player.Play();
                    break;
                }
                case "bigbox":
                {
                    var player = new SoundPlayer(Resources.bigbox);
                    player.Play();
                    break;
                }
                case "messagebox":
                {
                    var player = new SoundPlayer(Resources.messagebox);
                    player.Play();
                    break;
                }
            }
        }

        public static void ShowToast(string toast, Color color, string sound = null)
        {
            Client.StartAction(() =>
            {
                Border border = null;

                UiAction(() =>
                {
                    PlaySound(sound);
                    border = new Border
                    {
                        Background = Application.Current.Resources["BackgroundColor1"] as Brush,
                        CornerRadius = new CornerRadius(8),
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Height = 35
                    };
                    var textBlock = new TextBlock
                    {
                        Text = toast,
                        Foreground = new SolidColorBrush(color),
                        Margin = new Thickness(10)
                    };

                    border.Child = textBlock;
                    foreach (var control in MainWindow.ToastPanel.Children)
                    {
                        if (control is Border aaa)
                        {
                            aaa.Opacity -= 0.1;
                        }
                    }

                    MainWindow.ToastPanel.Children.Add(border);
                });
//                for (var i = 0; i < 35; i++)
//                {
//                    UiAction(() => { border.Height += 1; });
//                    Thread.Sleep(3);
//                }

                Thread.Sleep(3000);
                for (var i = 0; i < 50; i++)
                {
                    UiAction(() => { border.Opacity -= 0.02; });
                    Thread.Sleep(20);
                }

                //移除添加的对象
                UiAction(() => { MainWindow.ToastPanel.Children.Remove(border); });
            });
        }

        public static void ShowToast(string toast)
        {
            ShowToast(toast, Colors.White);
        }

        public static void UiAction(Action callback, bool isBegin = true)
        {
            try
            {
                if (isBegin)
                {
                    MainWindow?.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback);
                }
                else
                {
                    MainWindow?.Dispatcher.Invoke(DispatcherPriority.Background, callback);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static bool _isShowMessage;

        public static void ShowMessageControl(UserControl myControl)
        {
            MainWindow.MessageContent.Content = myControl;
            MainWindow.MessageContentBorader.Visibility = Visibility.Visible;
            _isShowMessage = true;
            //这里是用于堵塞ui达到showdialog的效果
            var frame = new DispatcherFrame();
            new Thread(() =>
            {
                while (_isShowMessage)
                {
                    Thread.Sleep(2);
                }

                frame.Continue = false;
            }).Start();
            Dispatcher.PushFrame(frame);
        }


        public static void CloseMessageControl()
        {
            MainWindow.MessageContent.Content = null;
            MainWindow.MessageContentBorader.Visibility = Visibility.Collapsed;
            _isShowMessage = false;
        }

        public static void SaveJson(string name, JObject json)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "xiakeyun");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            File.WriteAllText(Path.Combine(specificFolder, name), json.ToString(), Encoding.UTF8);
        }
        public static void DeleteJson(string name)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "xiakeyun");
            if (File.Exists(Path.Combine(specificFolder, name)))
            {
                File.Delete(Path.Combine(specificFolder, name));
            }
        }

        public static JObject LoadJson(string name)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "xiakeyun");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            return File.Exists(Path.Combine(specificFolder, name))
                ? JsonConvert.DeserializeObject<JObject>(
                    File.ReadAllText(Path.Combine(specificFolder, name), Encoding.UTF8))
                : null;
        }
    }
}