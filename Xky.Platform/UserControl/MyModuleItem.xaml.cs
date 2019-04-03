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
