using System;
using System.Windows;

namespace Xky.Core.UserControl
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyMessageBox : System.Windows.Controls.UserControl
    {
        public MyMessageBox(MessageBoxButton button,string text_yes="是", string text_no="否",string text_ok="确定")
        {
            InitializeComponent();
            BtnNo.Text = text_no;
            BtnOk.Text = text_ok;
            BtnYes.Text = text_yes;
            BtnOk.Click += BtnOk_Click;
            BtnYes.Click += BtnYes_Click;
            BtnNo.Click += BtnNo_Click;
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
                case MessageBoxButton.OKCancel:
                    BtnOk.Focusable = true;
                    BtnYes.Visibility = Visibility.Collapsed;
               
                    break;
            }

            DataContext = this;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Client.CloseDialogPanel();
           
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Client.CloseDialogPanel();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Client.CloseDialogPanel();
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