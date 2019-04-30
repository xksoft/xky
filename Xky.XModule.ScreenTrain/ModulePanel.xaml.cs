using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.XModule.ScreenTrain
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public Device device;
        bool isPaused = false;
        public ModulePanel()
        {
            InitializeComponent();
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Image_Screen.Source = device.ScreenShot;

        }

        private void Button_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                Image_Screen.Source = device.ScreenShot;
                isPaused = false;
                Button_Pause.Text = "暂停";
            }
            else
            {
                Image_Screen.Source = device.ScreenShot.Clone();
                isPaused = true;
                Button_Pause.Text = "继续";
            }

        }
        private bool _started;

        private Point _downPoint;
        private void Image_Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _started = true;

            _downPoint = e.GetPosition(Image_Screen);
        }

        private void Image_Screen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _started = false;
        }

        private void Image_Screen_MouseMove(object sender, MouseEventArgs e)
        {
            if (_started)
            {
                var point = e.GetPosition(Image_Screen);

                var rect = new Rect(_downPoint, point);
                Rectangle_Select.SetValue(Canvas.LeftProperty, rect.Left-1);
                Rectangle_Select.SetValue(Canvas.TopProperty, rect.Top-1);
                Rectangle_Select.Width = rect.Width;
                Rectangle_Select.Height = rect.Height;
            }
        }
    }
}
