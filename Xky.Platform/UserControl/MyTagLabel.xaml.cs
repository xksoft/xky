using System.Windows;
using System.Windows.Media;

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MyTagLabel.xaml 的交互逻辑
    /// </summary>
    public partial class MyTagLabel
    {
        public MyTagLabel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Brush TabLabelForeground
        {
            get => (Brush) GetValue(TabLabelForegroundProperty);
            set => SetValue(TabLabelForegroundProperty, value);
        }

        public static readonly DependencyProperty TabLabelForegroundProperty =
            DependencyProperty.Register("TabLabelForeground", typeof(Brush), typeof(MyTagLabel),
                new PropertyMetadata(new SolidColorBrush(Colors.Lime), OnColorChanged));

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var my = d as MyTagLabel;
            var color = ((SolidColorBrush) my.TabLabelForeground).Color;
            color.A = 30;
            my.TabLabelBackground = new SolidColorBrush(color);
        }



        public Brush TabLabelBackground
        {
            get => (Brush)GetValue(TabLabelBackgroundProperty);
            set => SetValue(TabLabelBackgroundProperty, value);
        }

        public static readonly DependencyProperty TabLabelBackgroundProperty =
            DependencyProperty.Register("TabLabelBackground", typeof(Brush), typeof(MyTagLabel),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(33, 0, 255, 0))));


        public string TabLabelText
        {
            get => (string) GetValue(TabLabelTextProperty);
            set => SetValue(TabLabelTextProperty, value);
        }

        public static readonly DependencyProperty TabLabelTextProperty =
            DependencyProperty.Register("TabLabelText", typeof(string), typeof(MyTagLabel),
                new PropertyMetadata("测试字符"));
    }
}