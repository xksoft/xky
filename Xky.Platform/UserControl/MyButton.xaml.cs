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

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MyButton.xaml 的交互逻辑
    /// </summary>
    public partial class MyButton 
    {
        public MyButton()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string ButtonText
        {
            get => (string)GetValue(ButtonTexttProperty);
            set => SetValue(ButtonTexttProperty, value);
        }

        public static readonly DependencyProperty ButtonTexttProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(MyTextBox),
                new PropertyMetadata(null));

        public Brush ButtonBackground1
        {
            get => (Brush)GetValue(ButtonBackground1Property);
            set => SetValue(ButtonBackground1Property, value);
        }

        public static readonly DependencyProperty ButtonBackground1Property =
            DependencyProperty.Register("ButtonBackground1", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(55,61,69))));

        public Brush ButtonBackground2
        {
            get => (Brush)GetValue(ButtonBackground2Property);
            set => SetValue(ButtonBackground2Property, value);
        }

        public static readonly DependencyProperty ButtonBackground2Property =
            DependencyProperty.Register("ButtonBackground2", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(65, 71, 79))));

        public Brush ButtonBackground3
        {
            get => (Brush)GetValue(ButtonBackground3Property);
            set => SetValue(ButtonBackground3Property, value);
        }

        public static readonly DependencyProperty ButtonBackground3Property =
            DependencyProperty.Register("ButtonBackground3", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(85, 91, 99))));

        public Brush ButtonForeground
        {
            get => (Brush)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register("ButtonForeground", typeof(Brush), typeof(MyTextBox),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));
    }
}
