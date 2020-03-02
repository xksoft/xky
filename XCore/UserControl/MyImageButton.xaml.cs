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

namespace XCore.UserControl
{
    /// <summary>
    /// MyImageButton.xaml 的交互逻辑
    /// </summary>
    public partial class MyImageButton : System.Windows.Controls.UserControl
    {
       


        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(MyImageButton),
                new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty ImageProperty =
           DependencyProperty.Register("Image", typeof(ImageSource), typeof(MyImageButton),
               new PropertyMetadata(null, OnImageChanged));



        public static readonly DependencyProperty Image_MouseOverProperty =
           DependencyProperty.Register("Image_MouseOver", typeof(ImageSource), typeof(MyImageButton),
               new PropertyMetadata(null));

        public static readonly DependencyProperty Image_PressedProperty =
           DependencyProperty.Register("Image_Pressed", typeof(ImageSource), typeof(MyImageButton),
               new PropertyMetadata(null));

        public static readonly DependencyProperty Image_HeightProperty =
          DependencyProperty.Register("Image_Height", typeof(int), typeof(MyImageButton),
              new PropertyMetadata(16));
        public static readonly DependencyProperty Image_WidthProperty =
          DependencyProperty.Register("Image_Width", typeof(int), typeof(MyImageButton),
              new PropertyMetadata(16));

        public static readonly DependencyProperty CornerRadiusProperty =
         DependencyProperty.Register("CornerRadius", typeof(int), typeof(MyImageButton),
             new PropertyMetadata(0));

        public static readonly DependencyProperty Background_MouseOverProperty =
            DependencyProperty.Register("Background_MouseOver", typeof(Brush), typeof(MyImageButton),
                new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty Background_PressedProperty =
            DependencyProperty.Register("Background_Pressed", typeof(Brush), typeof(MyImageButton),
                new PropertyMetadata(Brushes.Transparent));

        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(MyImageButton),
                new PropertyMetadata(Brushes.Transparent));

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyImageButton));


        public MyImageButton()
        {
            InitializeComponent();
            DataContext = this;
        }



        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);

        }
        public static void OnImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (((MyImageButton)obj).Image_MouseOver == null)
            {

                obj.SetValue(Image_MouseOverProperty, e.NewValue);
            }
            if (((MyImageButton)obj).Image_Pressed == null)
            {

                obj.SetValue(Image_PressedProperty, e.NewValue);
            }
        }
        public ImageSource Image_MouseOver
        {
            get => (ImageSource)GetValue(Image_MouseOverProperty);
            set => SetValue(Image_MouseOverProperty, value);

        }
        public ImageSource Image_Pressed
        {
            get => (ImageSource)GetValue(Image_PressedProperty);
            set => SetValue(Image_PressedProperty, value);
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
        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public Brush Background_MouseOver
        {
            get => (Brush)GetValue(Background_MouseOverProperty);
            set => SetValue(Background_MouseOverProperty, value);
        }

        public Brush Background_Pressed
        {
            get => (Brush)GetValue(Background_PressedProperty);
            set => SetValue(Background_PressedProperty, value);
        }

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }



        public void Button_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
