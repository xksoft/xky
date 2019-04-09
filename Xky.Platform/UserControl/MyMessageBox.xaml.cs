using System.Windows;

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyMessageBox : System.Windows.Controls.UserControl
    {
        public MyMessageBox()
        {
            InitializeComponent();
            BtnOk.Button1.Click += BtnOk_Click;
            BtnYes.Button1.Click += BtnYes_Click;
            BtnNo.Button1.Click += BtnNo_Click;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Common.CloseMessageControl();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Common.CloseMessageControl();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Common.CloseMessageControl();
        }

        public MessageBoxResult Result = MessageBoxResult.None;
    }
}