using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    public partial class ModulePanel : System.Windows.Controls.UserControl
    { 
        public static string AppPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        string DataPath = AppPath + "\\data";
        public Device device;
        Rect rect_select;
        private bool _started;
        private System.Windows.Point _downPoint;
        ImageSource imageSource;
        Queue transq = new Queue();
        int index = 0;
        EncoderParameters eps = new EncoderParameters(1);
        EncoderParameter ep_50 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
        EncoderParameter ep_100 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
        ImageCodecInfo jpsEncodeer = GetEncoder(ImageFormat.Jpeg);
        public class FileTimeInfo
        {
            public string FileName;  //文件名
            public DateTime FileCreateTime; //创建时间
        }
        static FileTimeInfo GetLatestFileTimeInfo(string dir, string ext)
        {
            List<FileTimeInfo> list = new List<FileTimeInfo>();
            DirectoryInfo d = new DirectoryInfo(dir);
            foreach (FileInfo file in d.GetFiles())
            {
                if (file.Extension.ToUpper() == ext.ToUpper())
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        FileCreateTime = file.CreationTime
                    });
                }
            }
            var f = from x in list
                    orderby x.FileCreateTime
                    select x;
            if (f == null) { return null; }
            return f.LastOrDefault();
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }
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

            if (!Directory.Exists(DataPath))
            {

                Directory.CreateDirectory(DataPath);

            }
            FileTimeInfo info = GetLatestFileTimeInfo(AppPath + "\\data", ".jpg");
            if (info == null) { index = 1; }
            else
            {
                FileInfo finfo = new FileInfo(info.FileName);
                index = Convert.ToInt32(finfo.Name.Remove((finfo.Name.LastIndexOf("."))));
                index++;
            }
            Image_Screen.Source = device.ScreenShot.Clone();

        }

        private void Button_Pause_Click(object sender, RoutedEventArgs e)
        {

            Image_Screen.Source = device.ScreenShot.Clone();
            index=index+2;

        }
 
        private void Image_Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {


                _started = true;

                _downPoint = e.GetPosition(Image_Screen);
                Console.WriteLine("Point:" + (_downPoint.X / Image_Screen.ActualWidth) + "  " + (_downPoint.Y / Image_Screen.ActualHeight));
            }
            else
            {
                _started = false;
                rect_select = new Rect(0, 0, 0, 0);
                Rectangle_Select.Width = Rectangle_Select.Height = 0;
            }
        }

        private void Image_Screen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _started = false;
                if (rect_select != null && rect_select.Width > 0 & rect_select.Height > 0)
                {
                     imageSource = Image_Screen.Source.Clone();
                    BitmapSource bitmapSource = ImageHelper.CutImage((BitmapSource)imageSource, new Int32Rect((int)rect_select.X, (int)rect_select.Y, (int)rect_select.Width, (int)rect_select.Height));

                    Image_Select.Source = bitmapSource;
                    Console.WriteLine("X:"+rect_select.X+" Y:"+rect_select.Y+" "+(rect_select.X/imageSource.Width)+"  "+ (rect_select.Y / imageSource.Height));
                }
                else { Image_Select.Source = null; }
            }
        }

        private void Image_Screen_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_started)
            {
                var point = e.GetPosition(Image_Screen);

                rect_select = new Rect(_downPoint, point);
                Rectangle_Select.SetValue(Canvas.LeftProperty, rect_select.Left - 1);
                Rectangle_Select.SetValue(Canvas.TopProperty, rect_select.Top - 1);
                Rectangle_Select.Width = rect_select.Width;
                Rectangle_Select.Height = rect_select.Height;
            }
        }

       
        private void Button_Train_Click(object sender, RoutedEventArgs e)
        {
            if (Image_Select.Source != null)
            {
               
                Bitmap bitmap = ImageHelper.ImageSourceToBitmap((BitmapSource)imageSource);
                eps.Param[0] = ep_100;
                bitmap.Save(DataPath + "\\" + index + ".jpg",jpsEncodeer,eps);
                File.AppendAllText(DataPath + "\\" + index + ".txt", ComboBox_Names.SelectedIndex + " " + (rect_select.Left + (rect_select.Width / 2)) / bitmap.Width + " " + (rect_select.Top + (rect_select.Height / 2)) / bitmap.Height + " " + rect_select.Width / bitmap.Width + " " + rect_select.Height / bitmap.Height + "\r\n");
                
                eps.Param[0] = ep_50;
                bitmap.Save(DataPath + "\\" +(index+1) + ".jpg", jpsEncodeer, eps);
                File.AppendAllText(DataPath + "\\" + (index+1) + ".txt", ComboBox_Names.SelectedIndex + " " + (rect_select.Left + (rect_select.Width / 2)) / bitmap.Width + " " + (rect_select.Top + (rect_select.Height / 2)) / bitmap.Height + " " + rect_select.Width / bitmap.Width + " " + rect_select.Height / bitmap.Height + "\r\n");
                
                

                Image_Select.Source = null;
                
            }
        }

        private void Image_Screen_SourceUpdated(object sender, DataTransferEventArgs e)
        {
           
        }

        private void Button_LoadNames_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "名称文本文件|*.txt";
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] names = File.ReadAllLines(openFileDialog.FileName,Encoding.UTF8);
                ComboBox_Names.ItemsSource = names;
            }
        }
     
        private void Button_OpenDir_Click(object sender, RoutedEventArgs e)
        {
           
            Process.Start(DataPath);

        }

      
       
    }
}
