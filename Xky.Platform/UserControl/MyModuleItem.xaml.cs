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

        public static readonly DependencyProperty ModuleNameProperty = DependencyProperty.Register(
            "ModuleName", typeof(string), typeof(MyModuleItem), new PropertyMetadata("未命名模块"));

        public string ModuleName
        {
            get => (string)GetValue(ModuleNameProperty);
            set => SetValue(ModuleNameProperty, value);
        }

    }
}
