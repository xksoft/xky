using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MyTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyTextBox : System.Windows.Controls.UserControl
    {
        public MyTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }




        /// <summary>
        /// 输入圆弧角度
        /// </summary>
        public static readonly DependencyProperty TextBoxCornerRadiusProperty = DependencyProperty.Register(
            "TextBoxCornerRadius", typeof(CornerRadius), typeof(MyTextBox), new PropertyMetadata(new CornerRadius(5)));

        public CornerRadius TextBoxCornerRadius
        {
            get => (CornerRadius) GetValue(TextBoxCornerRadiusProperty);
            set => SetValue(TextBoxCornerRadiusProperty, value);
        }

        public Brush TextBoxBackground
        {
            get => (Brush)GetValue(TextBoxBackgroundProperty);
            set => SetValue(TextBoxBackgroundProperty, value);
        }

        public static readonly DependencyProperty TextBoxBackgroundProperty =
            DependencyProperty.Register("TextBoxBackground", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush TextBoxForeground
        {
            get => (Brush)GetValue(TextBoxForegroundProperty);
            set => SetValue(TextBoxForegroundProperty, value);
        }

        public static readonly DependencyProperty TextBoxForegroundProperty =
            DependencyProperty.Register("TextBoxForeground", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public string TextBoxText
        {
            get => (string)GetValue(TextBoxProperty);
            set => SetValue(TextBoxProperty, value);
        }

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(null));

        public string TextBoxWaterText
        {
            get => (string)GetValue(TextBoxWaterTextProperty);
            set => SetValue(TextBoxWaterTextProperty, value);
        }

        public static readonly DependencyProperty TextBoxWaterTextProperty =
            DependencyProperty.Register("TextBoxWaterText", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(null));


        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            WaterLabel.Visibility = TextBox1.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}