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
using Newtonsoft.Json.Linq;
using Xky.Core;

namespace Xky.XModule.Demo
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class Panel : UserControl
    {
        public Panel()
        {
          
            InitializeComponent();
        }

        public TestModule Module;
        private void btn_file_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".txt";
            ofd.Filter = "txt file|*.txt";
            if (ofd.ShowDialog() == true)
            {
                text_filename.Text = ofd.FileName;
            }

        }
        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            if (text_filename.Text.Length == 0)
            {
                MessageBox.Show("请选择文件！");
            }
            else { Client.CloseDialogPanel();}
            
        }
        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            
         Client.CloseDialogPanel(); 

        }
    }
}
