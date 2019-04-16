using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xky.Core.UserControl
{
    /// <summary>
    ///     MyTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyTextBox : System.Windows.Controls.UserControl
    {
        /// <summary>
        ///     输入圆弧角度
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(MyTextBox), new PropertyMetadata(new CornerRadius(5)));

        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(55,61,69))));

        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public  static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(""));

        public static readonly DependencyProperty WaterTextProperty =
            DependencyProperty.Register("WaterText", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(null));

        public  static readonly DependencyProperty AcceptsReturnProperty =
           DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(MyTextBox),
               new PropertyMetadata(false));

        public  static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
           DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(MyTextBox),
               new PropertyMetadata(ScrollBarVisibility.Hidden));
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
          DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(MyTextBox),
              new PropertyMetadata(ScrollBarVisibility.Hidden));

        public static readonly DependencyProperty LineHeightProperty =
         DependencyProperty.Register("LineHeight", typeof(int), typeof(MyTextBox),
             new PropertyMetadata(10));

        public MyTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public new Brush Background
        {
            get => (Brush) GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public Brush Background_Hover
        {
            get
            {
                var color = ((SolidColorBrush) Background).Color;
                color = Color.FromRgb((byte) (color.R + 10), (byte) (color.G + 10), (byte) (color.B + 10));
                return new SolidColorBrush(color);
            }
        }


        public new Brush Foreground
        {
            get => (Brush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string WaterText
        {
            get => (string) GetValue(WaterTextProperty);
            set => SetValue(WaterTextProperty, value);
        }

        public bool AcceptsReturn
        {
            get => (bool)GetValue(AcceptsReturnProperty);
            set => SetValue(AcceptsReturnProperty, value);
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        public int LineHeight
        {
            get => (int)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }


        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            WaterLabel.Visibility = TextBox1.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            var color = ((SolidColorBrush) Background).Color;
            color = Color.FromRgb((byte) (color.R + 10), (byte) (color.G + 10), (byte) (color.B + 10));
            Background = new SolidColorBrush(color);
        }

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            var color = ((SolidColorBrush) Background).Color;
            color = Color.FromRgb((byte) (color.R - 10), (byte) (color.G - 10), (byte) (color.B - 10));
            Background = new SolidColorBrush(color);
        }
    }
}