using System;
using System.Collections.Generic;
using System.Linq;
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
using XCore;


namespace Xky.Platform.Pages
{
    /// <summary>
    /// Log.xaml 的交互逻辑
    /// </summary>
    public partial class Log : UserControl
    {
        public Log()
        {
            InitializeComponent();
           
        }
        public bool Stop = false;
        List<XCore.Model.Log> LogList = new List<XCore.Model.Log>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LogListBox.ItemsSource = LogList;

            Client.StartAction(() =>
            {
                while (this.IsVisible)
                {
                   
                    if (!Stop)
                    {
                      
                        lock ("Log")
                        {
                            LogList = (from l in Client.Logs select l).ToList();
                        }
                        Common.UiAction(() =>
                        {
                            LogListBox.ItemsSource = LogList;
                        });

                    }
                    Thread.Sleep(2000);
                }
                
            });

        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (Stop) { Stop = false; Button_Stop.Text = "停止刷新"; }
            else { Stop = true;

                Button_Stop.Text = "继续刷新";
            }
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            lock ("Log")
            {
                Client.Logs.Clear();
                LogList.Clear();
                LogListBox.ItemsSource=null;
            }
        }
    }
}
