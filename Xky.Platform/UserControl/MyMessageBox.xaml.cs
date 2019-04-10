using System;
using System.Windows;

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyMessageBox : System.Windows.Controls.UserControl
    {
        public MyMessageBox(MessageBoxButton button)
        {
            InitializeComponent();
            BtnOk.Button1.Click += BtnOk_Click;
            BtnYes.Button1.Click += BtnYes_Click;
            BtnNo.Button1.Click += BtnNo_Click;
            switch (button)
            {
                case MessageBoxButton.OK:
                    BtnOk.Focusable = true;
                    BtnYes.Visibility = Visibility.Collapsed;
                    BtnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    BtnYes.Focusable = true;
                    BtnOk.Visibility = Visibility.Collapsed;
                    break;
            }

            DataContext = this;
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

        public string MessageText
        {
            get => (string)GetValue(MessageTextProperty);
            set => SetValue(MessageTextProperty, value);
        }

        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(MyMessageBox),
                new PropertyMetadata("hello world"));
    }
}