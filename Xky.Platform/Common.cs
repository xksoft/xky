using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Xky.Platform
{
    public static class Common
    {
        public static MainWindow MainWindow;

        public static void ShowToast(string toast, Color color)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.ToastText.Text = toast;
                MainWindow.ToastText.Foreground = new SolidColorBrush(color);

                var showAnimation = new DoubleAnimation {From = 0, To = 1, FillBehavior = FillBehavior.HoldEnd};
                var start = new Storyboard {Duration = TimeSpan.FromSeconds(1)};
                start.Children.Add(showAnimation);
                Storyboard.SetTargetProperty(showAnimation, new PropertyPath("Opacity"));

                var holdAnimation = new DoubleAnimation {From = 1, To = 1, FillBehavior = FillBehavior.HoldEnd};
                var hold = new Storyboard {Duration = TimeSpan.FromSeconds(4)};
                hold.Children.Add(holdAnimation);
                Storyboard.SetTargetProperty(holdAnimation, new PropertyPath("Opacity"));

                var endAnimation = new DoubleAnimation {From = 1, To = 0, FillBehavior = FillBehavior.HoldEnd};
                var end = new Storyboard {Duration = TimeSpan.FromSeconds(1)};
                end.Children.Add(endAnimation);
                Storyboard.SetTargetProperty(endAnimation, new PropertyPath("Opacity"));
                if (MainWindow.ToastPanel.Opacity > 0)
                {
                   hold.Remove(MainWindow.ToastPanel);
                    hold.Completed += delegate
                    {
                        if (MainWindow.ToastText.Text == toast)
                            end.Begin(MainWindow.ToastPanel);
                    };
                    hold.Begin(MainWindow.ToastPanel);
                }
                else
                {
                    start.Completed += delegate
                    {
                        hold.Completed += delegate
                        {
                            if (MainWindow.ToastText.Text == toast)
                                end.Begin(MainWindow.ToastPanel);
                        };
                        hold.Begin(MainWindow.ToastPanel);
                    };
                    start.Begin(MainWindow.ToastPanel);
                }
            });
        }

        public static void ShowToast(string toast)
        {
            ShowToast(toast, Colors.White);
        }
    }
}