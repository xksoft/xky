using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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
        Rect rect_select;
        public ModulePanel()
        {
            InitializeComponent();
        }
        private string PostData(string postUrl, byte[] data)
        {
            string result = string.Empty;
            try
            {

                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = data.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(data, 0, data.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
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

        private System.Windows.Point _downPoint;
        private void Image_Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _started = true;

            _downPoint = e.GetPosition(Image_Screen);
        }

        private void Image_Screen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _started = false;
            if (rect_select != null && rect_select.Width > 0 & rect_select.Height > 0)
            {
                ImageSource imageSource = Image_Screen.Source.Clone();
                BitmapSource bitmapSource = ImageHelper.CutImage((BitmapSource)imageSource, new Int32Rect((int)rect_select.X, (int)rect_select.Y, (int)rect_select.Width, (int)rect_select.Height));

                Image_Select.Source = bitmapSource;
            }
            else { Image_Select.Source = null; }
        }

        private void Image_Screen_MouseMove(object sender, MouseEventArgs e)
        {
            if (_started)
            {
                var point = e.GetPosition(Image_Screen);

                rect_select = new Rect(_downPoint, point);
                Rectangle_Select.SetValue(Canvas.LeftProperty, rect_select.Left-1);
                Rectangle_Select.SetValue(Canvas.TopProperty, rect_select.Top-1);
                Rectangle_Select.Width = rect_select.Width;
                Rectangle_Select.Height = rect_select.Height;
            }
        }
       

        private void Button_Train_Click(object sender, RoutedEventArgs e)
        {
            if (Image_Select.Source != null)
            {
                Bitmap bitmap = ImageHelper.ImageSourceToBitmap(Image_Select.Source);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                PostData("http://192.168.1.66", ms.GetBuffer());
                ms.Close();
                Image_Select.Source = null;
            }
        }
    }
}
