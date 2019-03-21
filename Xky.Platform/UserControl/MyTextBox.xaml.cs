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

        private readonly MyTextBoxModel _myTextBoxModel = new MyTextBoxModel();

        public CornerRadius CornerRadius
        {
            get => (CornerRadius) GetValue(CornerRadiusProperty);

            set
            {
                SetValue(CornerRadiusProperty, value);
                _myTextBoxModel.CornerRadius = value;
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MyTabItem),
                new PropertyMetadata(new CornerRadius(5), null));

        public Brush MyBackground
        {
            get => _myTextBoxModel.Background;

            set
            {
                SetValue(MyBackgroundProperty, value);
                _myTextBoxModel.Background = value;
            }
        }

        public static readonly DependencyProperty MyBackgroundProperty =
            DependencyProperty.Register("MyBackground", typeof(Brush), typeof(MyTabItem),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), null));

        public Brush MyForeground
        {
            get => _myTextBoxModel.Foreground;

            set
            {
                SetValue(MyForegroundProperty, value);
                _myTextBoxModel.Foreground = value;
            }
        }

        public static readonly DependencyProperty MyForegroundProperty =
            DependencyProperty.Register("MyForeground", typeof(Brush), typeof(MyTabItem),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), null));

        public string Text
        {
            get => _myTextBoxModel.Text;

            set
            {
                SetValue(TextProperty, value);
                _myTextBoxModel.Text = value;
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyTabItem),
                new PropertyMetadata(null, null));

        public string WaterText
        {
            get => _myTextBoxModel.WaterText;

            set
            {
                SetValue(WaterTextProperty, value);
                _myTextBoxModel.WaterText = value;
            }
        }

        public static readonly DependencyProperty WaterTextProperty =
            DependencyProperty.Register("WaterText", typeof(string), typeof(MyTabItem),
                new PropertyMetadata(null, null));

        public class MyTextBoxModel : INotifyPropertyChanged
        {
            private CornerRadius _cornerRadius = new CornerRadius(5);

            public CornerRadius CornerRadius
            {
                get => _cornerRadius;
                set
                {
                    _cornerRadius = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CornerRadius"));
                }
            }

            private Brush _background = new SolidColorBrush(Colors.Black);

            public Brush Background
            {
                get => _background;
                set
                {
                    _background = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Background"));
                }
            }

            private Brush _foreground = new SolidColorBrush(Colors.White);

            public Brush Foreground
            {
                get => _foreground;
                set
                {
                    _foreground = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Foreground"));
                }
            }

            private string _text;

            public string Text
            {
                get => _text;
                set
                {
                    _text = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
                }
            }

            private string _waterText = "我是水印";

            public string WaterText
            {
                get => _waterText;
                set
                {
                    _waterText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WaterText"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            WaterLabel.Visibility = TextBox1.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}