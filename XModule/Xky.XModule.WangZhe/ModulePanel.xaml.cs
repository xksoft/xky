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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.XModule.WangZhe
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : System.Windows.Controls.UserControl
    {

        public Device device;
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
               Console.WriteLine(ex.Message);
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
            Image_Screen.Source.Changed += Source_Changed;

        }

        private void Source_Changed(object sender, EventArgs e)
        {
           
        }
    }
}
