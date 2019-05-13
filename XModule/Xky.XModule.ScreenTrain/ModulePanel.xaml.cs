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
        bool isPaused = false;
        bool isTrans = false;
        Rect rect_select;
        long lasttrans = 0;
        ImageSource imageSource;
        Queue transq = new Queue();
        int index = 0;
        bool waite = false;
        EncoderParameters eps = new EncoderParameters(1);
        EncoderParameter ep_20 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,20L);
        EncoderParameter ep_60 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
        EncoderParameter ep_100 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
        ImageCodecInfo jpsEncodeer = GetEncoder(ImageFormat.Jpeg);
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
                System.Windows.MessageBox.Show(ex.Message);
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
            StartTransQueue();
            if (!Directory.Exists(DataPath))
            {

                Directory.CreateDirectory(DataPath);

            }
            FileTimeInfo info= GetLatestFileTimeInfo(AppPath + "\\data", ".jpg");
            if (info == null) { index = 1; }
            else {     FileInfo finfo = new FileInfo(info.FileName);
                index = Convert.ToInt32(finfo.Name.Remove((finfo.Name.LastIndexOf("."))));
                index++;
            }
        
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
            if (e.ChangedButton == MouseButton.Left)
            {


                _started = true;

                _downPoint = e.GetPosition(Image_Screen);
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
                    Console.WriteLine("X:"+rect_select.X+" Y:"+rect_select.Y);
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
        private void Button_Train_Click(object sender, RoutedEventArgs e)
        {
            if (Image_Select.Source != null)
            {
               
                Bitmap bitmap = ImageHelper.ImageSourceToBitmap((BitmapSource)imageSource);
                eps.Param[0] = ep_100;
                bitmap.Save(DataPath + "\\" + index + ".jpg",jpsEncodeer,eps);
                File.AppendAllText(DataPath + "\\" + index + ".txt", ComboBox_Names.SelectedIndex + " " + (rect_select.Left + (rect_select.Width / 2)) / bitmap.Width + " " + (rect_select.Top + (rect_select.Height / 2)) / bitmap.Height + " " + rect_select.Width / bitmap.Width + " " + rect_select.Height / bitmap.Height + "\r\n");
                index++;
                eps.Param[0] = ep_60;
                bitmap.Save(DataPath + "\\" + index + ".jpg", jpsEncodeer, eps);
                File.AppendAllText(DataPath + "\\" + index + ".txt", ComboBox_Names.SelectedIndex + " " + (rect_select.Left + (rect_select.Width / 2)) / bitmap.Width + " " + (rect_select.Top + (rect_select.Height / 2)) / bitmap.Height + " " + rect_select.Width / bitmap.Width + " " + rect_select.Height / bitmap.Height + "\r\n");
                index++;
                eps.Param[0] = ep_20;
                bitmap.Save(DataPath + "\\" + index + ".jpg", jpsEncodeer, eps);
                File.AppendAllText(DataPath + "\\" + index + ".txt", ComboBox_Names.SelectedIndex + " " + (rect_select.Left + (rect_select.Width / 2)) / bitmap.Width + " " + (rect_select.Top + (rect_select.Height / 2)) / bitmap.Height + " " + rect_select.Width / bitmap.Width + " " + rect_select.Height / bitmap.Height + "\r\n");
                index++;

                Image_Select.Source = null;
                
            }
        }

        private void Button_Find_Click(object sender, RoutedEventArgs e)
        {

            if (isTrans)
            {
                isTrans = false;
                Button_Find.Text = "开始识别";
            }
            else
            {
                isTrans = true;
                Button_Find.Text = "停止识别";

            }



        }

        private void Source_Changed(object sender, EventArgs e)
        {
             if (DateTime.Now.Ticks - lasttrans >= 1000000 && isTrans)
            {
                eps.Param[0] = ep_20;
                lasttrans = DateTime.Now.Ticks;
                Bitmap bitmap = ImageHelper.ImageSourceToBitmap((BitmapSource)Image_Screen.Source);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, jpsEncodeer, eps);
                ms.Close();
                transq.Enqueue(ms.GetBuffer());

            }
        }
        public void StartTransQueue()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    if (transq.Count > 0)
                    {
                        object obj = transq.Dequeue();
                        if (obj != null)
                        {
                            string result = PostData("http://192.168.1.8/api?action=getobject", (byte[])obj);
                            Console.WriteLine(result);
                            TransResult tr = JsonConvert.DeserializeObject <TransResult>( result);
                            if (tr != null)
                            {
                              
                                List<TransItem> tritems = JsonConvert.DeserializeObject<List<TransItem>>(tr.msg);
                                var items = (from i in tritems where i.Type == "敌方防御塔" || i.Type == "敌方水晶" select i).ToList();
                                if (items.Count > 0)
                                {
                                    nodcount = 0;
                                    Console.WriteLine("后退");
                                    AiPlay_Back();

                                }
                                else
                                {
                                    items = (from i in tritems where i.Type == "敌方小兵" || i.Type == "敌人" select i).ToList();
                                    if (items.Count > 0)
                                    {
                                        nodcount = 0;
                                        aistate = "攻击";
                                        Console.WriteLine("攻击");
                                        device.ScriptEngine.AdbShell("input tap 1260 600&&input tap 1043 632&&input tap 1089 501&&input tap 1239 416&&input tap 132 298&&input tap 145 377");


                                    }

                                }
                                if (!waite) { AiPlay(tritems); }
                               
                                //Console.WriteLine(tritems.Count);
                                //Console.WriteLine("识别列队剩余：" + transq.Count);

                                for (int i = 0; i <5; i++)
                                {

                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        var fo = Canvas_Main.FindName("Rectangle_" + i);
                                        if (fo != null)
                                        {
                                            System.Windows.Shapes.Rectangle rectangle = fo as System.Windows.Shapes.Rectangle;
                                            if (tritems.Count >i)
                                            {
                                                if (tritems[i].Width<=500&& tritems[i].Height<=500) {
                                                    rectangle.SetValue(Canvas.LeftProperty, (double)tritems[i].X);
                                                    rectangle.SetValue(Canvas.TopProperty, (double)tritems[i].Y);
                                                    rectangle.Width = tritems[i].Width;
                                                    rectangle.Height = tritems[i].Height;
                                                }
                                            }
                                            else
                                            {
                                                rectangle.Width = rectangle.Height = 0;
                                            }
                                            
                                        }
                                    }));
                                }
                            }
                        }
                    }
                }
            })
            { IsBackground = true }.Start();
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
            if (f==null) { return null; }
            return f.LastOrDefault();
        }
        private void Button_OpenDir_Click(object sender, RoutedEventArgs e)
        {
           
            Process.Start(DataPath);

        }

        private void Button_Pause_Loaded(object sender, RoutedEventArgs e)
        {

        }
        string aistate = "在家";
        int nodcount = 0;
        public void AiPlay(List<TransItem> tritems)
        {
            if (waite)
            {
                return;
            }
            new Thread(() =>
            {
                waite = true;
                if (aistate == "在家")
                {
                  
                    var items = (from i in tritems where i.Type == "敌方防御塔" || i.Type == "敌方水晶" select i).ToList();
                    if (items.Count > 0)
                    {
                        nodcount = 0;
                        Console.WriteLine("后退");
                        AiPlay_Back();
                        waite = false;
                        return;
                    }

                    items = (from i in tritems where i.Type == "敌方小兵" || i.Type == "敌人" select i).ToList();
                    if (items.Count > 0)
                    {
                        nodcount = 0;
                        aistate = "攻击";
                        Console.WriteLine("攻击");
                        device.ScriptEngine.AdbShell("input tap 1260 600");
                        waite = false;
                        return;
                        
                    }

                    items = (from i in tritems where i.Type == "我方水晶"||i.Type=="我方防御塔"||i.Type=="老家" select i).ToList();
                    if (items.Count() > 0)
                    {
                        //Thread.Sleep(5000);
                        Console.WriteLine("出门");
                        AiPlay_Go();
                    }
                    else
                    {
                      
                        Console.WriteLine("出门");
                        AiPlay_Go();
                    }
                   
                }
                else if (aistate == "攻击")
                {
                
                   var items = (from i in tritems where i.Type == "敌方防御塔" ||i.Type == "敌方水晶" select i).ToList();
                    if (items.Count > 0)
                    {
                        nodcount = 0;

                        Console.WriteLine("后退");
                        AiPlay_Back();
                        waite = false;
                        return;
                    }

                    items = (from i in tritems where i.Type == "敌方小兵" || i.Type == "敌人" select i).ToList();
                    if (items.Count > 0)
                    {
                        nodcount = 0;
                        aistate = "攻击";
                        Console.WriteLine("攻击");
                        device.ScriptEngine.AdbShell("input tap 1260 600&&input tap 1043 632&&input tap 1089 501&&input tap 1239 416&&input tap 132 298&&input tap 145 377");
                     

                    }
                    else {

                        items = (from i in tritems where i.Type == "我方水晶"||i.Type=="老家" select i).ToList();
                        if (items.Count > 0)
                        {
                            aistate = "在家";

                        }
                        else {
                           // 
                            //没有敌人了，前进
                            nodcount++;
                            Thread.Sleep(1000);
                            if (nodcount >= 5)
                            {Console.WriteLine("没有敌方单位了，前进");

                                device.ScriptEngine.AdbShell("input swipe 294 578 333 497 1000");
                                nodcount = 0;
                            }
                       
                        }
                      
                    }

                    
                }
                waite = false;
            })
            { IsBackground = true }.Start();
        }
        public void AiPlay_Go()
        {
         
                device.ScriptEngine.AdbShell("input swipe 294 578 333 497 5000");
          
        }
        public void AiPlay_Back()
        {

            device.ScriptEngine.AdbShell("input swipe 294 587 246 640 5000");

        }
    }
}
