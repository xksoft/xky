using System;
using System.IO;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Platform.Properties;
using Xky.Platform.UserControl.Pages;

namespace Xky.Platform
{
    public static class Common
    {
        public static MainWindow MainWindow;
        public static MyMainControl MyMainControl;

        private static int _showToastStep;


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
            Client.StartAction(async () =>
            {
                _showToastStep++;
                var currentStep = _showToastStep;
                var current = (double) 0;
                UiAction(() => { current = MainWindow.ToastPanel.Opacity; });
//                if (current > 0)
//                {
//                    for (var i = 0; i < 21; i++)
//                    {
//                        if (_showToastStep != currentStep)
//                            break;
//                        var i1 = i;
//                        UiAction(() => { MainWindow.ToastPanel.Opacity = current * (20 - i1) * 0.05; });
//                        await Task.Delay(1);
//                    }
//                }

                UiAction(() =>
                {
                    PlaySound(sound);
                    MainWindow.ToastText.Text = toast;
                    MainWindow.ToastText.Foreground = new SolidColorBrush(color);
                });
                for (var i = 0; i < 21; i++)
                {
                    if (_showToastStep != currentStep)
                        break;
                    var i1 = i;
                    UiAction(() => { MainWindow.ToastPanel.Opacity = current + i1 * 0.05; });
                    await Task.Delay(1);
                }

                await Task.Delay(3000);
                for (var i = 0; i < 26; i++)
                {
                    if (_showToastStep != currentStep)
                        break;
                    var i1 = i;
                    UiAction(() => { MainWindow.ToastPanel.Opacity = (25 - i1) * 0.04; });
                    await Task.Delay(3);
                }
            });
        }

        public static void ShowToast(string toast)
        {
            ShowToast(toast, Colors.White);
        }

        public static void UiAction(Action callback)
        {
            try
            {
                MainWindow?.Dispatcher.BeginInvoke(callback);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void ShowMessageControl(System.Windows.Controls.UserControl myControl)
        {

                MainWindow.MessageContent.Content = myControl;
                MainWindow.MessageContentBorader.Visibility = Visibility.Visible;
                MessageBox.Show("sdf");

        }

        public static void CloseMessageControl()
        {
            UiAction(() =>
            {
                MainWindow.MessageContent.Content = null;
                MainWindow.MessageContentBorader.Visibility = Visibility.Collapsed;
            });
        }

        public static void SaveJson(string name, JObject json)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "xiakeyun");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            File.WriteAllText(Path.Combine(specificFolder, name), json.ToString(), Encoding.UTF8);
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