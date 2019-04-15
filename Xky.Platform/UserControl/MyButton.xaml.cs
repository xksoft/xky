using System.Windows;
using System.Windows.Media;

namespace Xky.Platform.UserControl
{
    /// <summary>
    ///     MyButton.xaml 的交互逻辑
    /// </summary>
    public partial class MyButton
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyButton),
                new PropertyMetadata(null));

        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MyButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(55, 61, 69))));

        public  static readonly DependencyProperty ImageProperty =
           DependencyProperty.Register("Image", typeof(ImageSource), typeof(MyButton),
               new PropertyMetadata(null));

        public static readonly DependencyProperty Image_HeightProperty =
          DependencyProperty.Register("Image_Height", typeof(int), typeof(MyButton),
              new PropertyMetadata(16));
        public static readonly DependencyProperty Image_WidthProperty =
          DependencyProperty.Register("Image_Width", typeof(int), typeof(MyButton),
              new PropertyMetadata(16));

        public static readonly DependencyProperty Background_MouseOverProperty =
            DependencyProperty.Register("Background_MouseOver", typeof(Brush), typeof(MyButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(65, 71, 79))));

        public static readonly DependencyProperty Background_PressedProperty =
            DependencyProperty.Register("Background_Pressed", typeof(Brush), typeof(MyButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(85, 91, 99))));

        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(MyButton),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyButton));

       
        public MyButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public new Brush Background
        {
            get => (Brush) GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
        public int Image_Height
        {
            get => (int)GetValue(Image_HeightProperty);
            set => SetValue(Image_HeightProperty, value);
        }
        public int Image_Width
        {
            get => (int)GetValue(Image_WidthProperty);
            set => SetValue(Image_WidthProperty, value);
        }
        public Brush Background_MouseOver
        {
            get => (Brush) GetValue(Background_MouseOverProperty);
            set => SetValue(Background_MouseOverProperty, value);
        }

        public Brush Background_Pressed
        {
            get => (Brush) GetValue(Background_PressedProperty);
            set => SetValue(Background_PressedProperty, value);
        }

        public new Brush Foreground
        {
            get => (Brush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

      
      
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(MyButton.ClickEvent);
            RaiseEvent(newEventArgs);
        }
    }
}