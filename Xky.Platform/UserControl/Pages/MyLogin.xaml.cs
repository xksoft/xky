using System;

namespace Xky.Platform.UserControl.Pages
{
    /// <summary>
    /// MyLogin.xaml 的交互逻辑
    /// </summary>
    public partial class MyLogin
    {
        public MyLogin()
        {
            InitializeComponent();
            BtnLogin.Button1.Click += Login_Click;
        }

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Common.ShowToast(DateTime.Now.ToString());
            Console.WriteLine("dianle");
        }
    }
}