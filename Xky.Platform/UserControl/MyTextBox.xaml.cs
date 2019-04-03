using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MyTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyTextBox
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
            get => (Brush) GetValue(TextBoxBackgroundProperty);
            set => SetValue(TextBoxBackgroundProperty, value);
        }

        public static readonly DependencyProperty TextBoxBackgroundProperty =
            DependencyProperty.Register("TextBoxBackground", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush HoverTextBoxBackground
        {
            get
            {
                var color = ((SolidColorBrush) TextBoxBackground).Color;
                color = Color.FromRgb((byte) (color.R + 10), (byte) (color.G + 10), (byte) (color.B + 10));
                return new SolidColorBrush(color);
            }
        }


        public Brush TextBoxForeground
        {
            get => (Brush) GetValue(TextBoxForegroundProperty);
            set => SetValue(TextBoxForegroundProperty, value);
        }

        public static readonly DependencyProperty TextBoxForegroundProperty =
            DependencyProperty.Register("TextBoxForeground", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public string TextBoxText
        {
            get => (string) GetValue(TextBoxProperty);
            set => SetValue(TextBoxProperty, value);
        }

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(null));

        public string TextBoxWaterText
        {
            get => (string) GetValue(TextBoxWaterTextProperty);
            set => SetValue(TextBoxWaterTextProperty, value);
        }

        public static readonly DependencyProperty TextBoxWaterTextProperty =
            DependencyProperty.Register("TextBoxWaterText", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(null));


        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            WaterLabel.Visibility = TextBox1.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            var color = ((SolidColorBrush) TextBoxBackground).Color;
            color = Color.FromRgb((byte) (color.R + 10), (byte) (color.G + 10), (byte) (color.B + 10));
            TextBoxBackground = new SolidColorBrush(color);
        }

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            var color = ((SolidColorBrush) TextBoxBackground).Color;
            color = Color.FromRgb((byte) (color.R - 10), (byte) (color.G - 10), (byte) (color.B - 10));
            TextBoxBackground = new SolidColorBrush(color);
        }
    }
}