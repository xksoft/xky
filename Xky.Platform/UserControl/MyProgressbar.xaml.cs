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
    /// MyProgressbar.xaml 的交互逻辑
    /// </summary>
    public partial class MyProgressbar
    {
        public MyProgressbar()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MyProgressBar_Loaded);
        }

        void MyProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(MyProgressbar), new PropertyMetadata(100d, OnMaximumChanged));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        private static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(MyProgressbar), new PropertyMetadata(Brushes.Black));
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

         public new  Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

       
        public new Brush Foreground
        {
            get {
                return (Brush)GetValue(ForegroundProperty);
            }
            set {
                SetValue(ForegroundProperty, value);
               
            }
        }

        private static readonly new DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(int), typeof(MyProgressbar), new PropertyMetadata(0));
        public new int BorderThickness
        {
            get { return (int)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
        private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(MyProgressbar), new PropertyMetadata(0d, OnMinimumChanged));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MyProgressbar), new PropertyMetadata(50d, OnValueChanged));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }



        private static readonly DependencyProperty ProgressBarWidthProperty = DependencyProperty.Register("ProgressBarWidth", typeof(double), typeof(MyProgressbar), null);
        private double ProgressBarWidth
        {
            get { return (double)GetValue(ProgressBarWidthProperty); }
            set { SetValue(ProgressBarWidthProperty, value); }
        }

        static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as MyProgressbar).Update();
        }

        static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as MyProgressbar).Update();
        }

        static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as MyProgressbar).Update();
        }


        void Update()
        {
            // The -2 is for the borders - there are probably better ways of doing this since you
            // may want your template to have variable bits like border width etc which you'd use
            // TemplateBinding for
            ProgressBarWidth = Math.Min((Value / (Maximum + Minimum) * this.ActualWidth) - 2, this.ActualWidth - 2);
            if (Value/Maximum>=0.5) {
                Foreground = Brushes.Red;
            }

        }
    }
}
