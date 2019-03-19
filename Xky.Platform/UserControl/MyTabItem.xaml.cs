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
    /// MyTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class MyTabItem : System.Windows.Controls.UserControl
    {
        public MyTabItem()
        {
            InitializeComponent();
            ItemList.Add(this);
        }

        private static readonly List<MyTabItem> ItemList = new List<MyTabItem>();

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MyTabItem), new PropertyMetadata(false,
                (o, e) =>
                {
                    var li = (MyTabItem) o;
                }));

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                foreach (var myTabItem in ItemList)
                {
                    myTabItem.IsSelected = myTabItem == this;
                }
            }
        }
    }
}