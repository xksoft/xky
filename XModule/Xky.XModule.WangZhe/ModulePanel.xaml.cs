using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using XCore;
using XCore.Model;

namespace Xky.XModule.WangZhe
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : System.Windows.Controls.UserControl
    {
        public bool Exit = false;
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
        private Queue transq = new Queue();//屏幕截图列队
        private Queue adbQueue = new Queue();
        private long lastDate = 0;//最后进入列队时间
        private Font nameFont = new Font("Arial", 9);
        private SolidBrush nameBrush = new SolidBrush(System.Drawing.Color.GreenYellow);
        private System.Drawing.Pen namePen = new System.Drawing.Pen(System.Drawing.Color.GreenYellow, 2);
        private Bitmap bitmapNull = new Bitmap(100, 100);
        private Graphics graphicsNull = null;
      
        public void StartTransQueue()
        {
            new Thread(() =>
            {
                while (true&&!Exit)
                {
                   
                    
                    Thread.Sleep(1);
                    if (transq.Count > 0)
                    {
                        Bitmap bitmap = (Bitmap)transq.Dequeue();
                        if (bitmap != null)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                bitmap.Save(stream, ImageFormat.Bmp);

                                stream.Position = 0;
                                BitmapImage result = new BitmapImage();
                                result.BeginInit();

                                result.CacheOption = BitmapCacheOption.OnLoad;
                                result.StreamSource = stream;
                                result.EndInit();
                                result.Freeze();
                                //提交图片并返回识别到的对象列表
                                string httpresult = PostData("http://192.168.1.8/api?action=getobject", stream.GetBuffer());
                                Console.WriteLine(httpresult);
                                TransResult tr = JsonConvert.DeserializeObject<TransResult>(httpresult);
                                if (tr != null)
                                {

                                    List<TransItem> transItems = JsonConvert.DeserializeObject<List<TransItem>>(tr.msg);
                                    if (!Playing) { AiPlay(transItems); }
                                   
                                    if (graphicsNull != null)
                                    {
                                        graphicsNull.Clear(System.Drawing.Color.Transparent);
                                    }
                                    if (transItems.Count > 0)
                                    {


                                        foreach (TransItem item in transItems)
                                        {

                                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(item.X, item.Y, item.Width, item.Height);

                                                graphicsNull.DrawRectangle(namePen, rect);
                                                graphicsNull.DrawString(item.Type, nameFont, nameBrush, item.X, item.Y);

                                        }


                                    }
                                  
                                    this.Dispatcher.Invoke(new Action(() => { Image_ScreenCopy.Source = ImageHelper.BitmapToBitmapImage(bitmapNull); }));
                                 

                                }


                            }


                        }
                    }
                }
            })
            { IsBackground = true }.Start();
        }
       
        public int safeCount = 0;
        public int buyCount = 0;
        public bool Playing = false;
        public void AiPlay(List<TransItem> tritems)
        {
            new Thread(() =>
            {
                if (Playing) { return; }
            Playing = true;
             var items = (from i in tritems where (i.Type == "敌方防御塔" || i.Type == "敌方水晶") select i).ToList();
            if (items.Count > 0)
            {
                safeCount = 0;
                Console.WriteLine("后退");
                device.ScriptEngine.MouseDown(0.2, 0.78);
                Thread.Sleep(50);
                device.ScriptEngine.MouseDrag(0.17, 0.864);
                Thread.Sleep(2000);
                device.ScriptEngine.MouseUp(0.17, 0.864);

            }
            else
            {
                items = (from i in tritems where (i.Type == "敌方小兵" || i.Type == "敌人") select i).ToList();
                    if (items.Count > 0 && items.Count < 4)
                    {
                        safeCount = 0;
                        Console.WriteLine("攻击");
                        device.ScriptEngine.Click(0.868, 0.844);
                        device.ScriptEngine.Click(0.716, 0.872);
                        device.ScriptEngine.Click(0.772, 0.682);
                        device.ScriptEngine.Click(0.858, 0.576);
                    }
                    else if (items.Count >= 4) {
                        //怂
                        Console.WriteLine("后退");
                        device.ScriptEngine.MouseDown(0.2, 0.78);
                        Thread.Sleep(50);
                        device.ScriptEngine.MouseDrag(0.17, 0.864);
                        Thread.Sleep(2000);
                        device.ScriptEngine.MouseUp(0.17, 0.864);
                    }
                    else
                    {

                        if (buyCount >= 10 || (from i in tritems where (i.Type == "装备购买提示" || i.Type == "技能升级提示") select i).ToList().Count > 0)
                        {
                            //一段时间内周围没有地方单位，购买装备，学习技能
                            device.ScriptEngine.Click(0.093, 0.393);
                            device.ScriptEngine.Click(0.091, 0.5);

                            device.ScriptEngine.Click(0.817, 0.476);
                            device.ScriptEngine.Click(0.66, 0.766);
                            device.ScriptEngine.Click(0.72, 0.57);

                       

                            buyCount = 0;
                        }
                        if (safeCount >= 10)
                        {
                            //前进
                            Console.WriteLine("前进");
                            device.ScriptEngine.MouseDown(0.2, 0.78);
                            Thread.Sleep(50);
                            device.ScriptEngine.MouseDrag(0.24, 0.67);
                            Thread.Sleep(800);
                            device.ScriptEngine.MouseUp(0.24, 0.67);
                            safeCount = 0;
                        }


                        //没有敌方单位
                        items = (from i in tritems where (i.Type == "我方小兵" || i.Type == "队友" || i.Type == "我方防御塔"||i.Type=="老家") select i).ToList();
                        if (items.Count > 0)
                        {
                            //有我方小兵或队友在的时候不要方
                            safeCount = safeCount + items.Count;
                            buyCount++;
                        }
                        else {
                            safeCount++;
                            buyCount++;
                        }
                    }

            }
            Playing = false;
            })
            { IsBackground = true }.Start();
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Image_Screen.Source = device.ScreenShot;


            Image_Screen.Source.Changed += Source_Changed;
            Image_ScreenCopy.Width = Image_Screen.Width;
            Image_ScreenCopy.Height = Image_Screen.Height;

            StartTransQueue();
           

        }

        private void Source_Changed(object sender, EventArgs e)
        {
            if (bitmapNull.Width==100)
            {
                bitmapNull = new Bitmap((int)Image_Screen.ActualWidth, (int)Image_Screen.ActualHeight);
                
                graphicsNull= Graphics.FromImage(bitmapNull);
                graphicsNull.SmoothingMode = SmoothingMode.AntiAlias;
            }
            if (DateTime.Now.Ticks - lastDate >= 2000000)
            {
                //控制一秒内最多5张图片进入列队 
                BitmapSource bitmapSource = (BitmapSource)device.ScreenShot.Clone();
                Bitmap bitmap = ImageHelper.ImageSourceToBitmap(bitmapSource);
                transq.Enqueue(bitmap);
                lastDate = DateTime.Now.Ticks;
            }



        }

    }
    public class TransResult
    {
        public int status;
        public string action = "";
        public string msg = "";
        public long count;
    }
    public class TransItem
    {

        public string Type { get; set; }
        public double Confidence { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
