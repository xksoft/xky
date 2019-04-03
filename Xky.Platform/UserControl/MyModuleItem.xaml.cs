using System.Windows;
using System.Windows.Media;

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MyModuleItem.xaml 的交互逻辑
    /// </summary>
    public partial class MyModuleItem 
    {
        public MyModuleItem()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 是否正在执行
        /// </summary>
        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
            "IsRunning", typeof(bool), typeof(MyModuleItem), new PropertyMetadata(false));

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public  static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(MyModuleItem), new PropertyMetadata("未命名模块"));

        public  string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public  static readonly DependencyProperty LogoProperty = DependencyProperty.Register(
            "Logo", typeof(string), typeof(MyModuleItem), new PropertyMetadata((ImageSource)null));

        public ImageSource Logo
        {
            get => (ImageSource)GetValue(LogoProperty);
            set => SetValue(LogoProperty, value);
        }

    }
}
