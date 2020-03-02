using System.Windows;
using System.Windows.Media;

namespace XCore.UserControl
{
    /// <summary>
    ///     MyTagLabel.xaml 的交互逻辑
    /// </summary>
    public partial class MyTagLabel : System.Windows.Controls.UserControl
    {
        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(MyTagLabel),
                new PropertyMetadata(new SolidColorBrush(Colors.Lime), OnColorChanged));

        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MyTagLabel),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(33, 0, 255, 0))));

        public  static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyTagLabel),
                new PropertyMetadata("测试字符"));

        //public static readonly DependencyProperty PaddingProperty =
        //   DependencyProperty.Register("Text", typeof(), typeof(MyTagLabel),
        //       new PropertyMetadata("测试字符"));

        public MyTagLabel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public new Brush Foreground
        {
            get => (Brush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }


        public new Brush Background
        {
            get => (Brush) GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }


        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var my = d as MyTagLabel;
            var color = ((SolidColorBrush) my.Foreground).Color;
            color.A = 30;
            my.Background = new SolidColorBrush(color);
        }
    }
}