using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xky.Core;
using File = System.IO.File;

namespace Xky.Platform
{
    public static class Common
    {
        public static MainWindow MainWindow;


        private static int _showToastStep = 0;

        public static void ShowToast(string toast, Color color)
        {
            Task.Factory.StartNew(async () =>
            {
                _showToastStep++;
                var currentStep = _showToastStep;
                var current = (double) 0;
                UiAction(() => { current = MainWindow.ToastPanel.Opacity; });
                if (current > 0)
                {
                    for (var i = 0; i < 21; i++)
                    {
                        if (_showToastStep != currentStep)
                            break;
                        var i1 = i;
                        UiAction(() => { MainWindow.ToastPanel.Opacity = current * (20 - i1) * 0.05; });
                        await Task.Delay(1);
                    }
                }

                UiAction(() =>
                {
                    MainWindow.ToastText.Text = toast;
                    MainWindow.ToastText.Foreground = new SolidColorBrush(color);
                });
                for (var i = 0; i < 21; i++)
                {
                    if (_showToastStep != currentStep)
                        break;
                    var i1 = i;
                    UiAction(() => { MainWindow.ToastPanel.Opacity = i1 * 0.05; });
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
            MainWindow?.Dispatcher.Invoke(callback);
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