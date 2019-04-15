using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Xky.Core;
using Xky.Core.Model;

namespace Xky.Platform.Pages
{
    /// <summary>
    /// MyNode.xaml 的交互逻辑
    /// </summary>
    public partial class Node 
    {
     
        public Node()
        {
        
            InitializeComponent();
           
            NodeListBox.ItemsSource = Client.AllNodes;
            
        }

      

        private void NodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NodeListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }
    }

    public partial class ConnectStatusConvertToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            int connectstatus = (int)value;
            if (connectstatus == 0)
            {
                return new Uri( "/Xky.Platform;component/Resources/Icon/lannode_online.png", UriKind.Absolute);
            }
            else
            {
                return new Uri("/Xky.Platform;component/Resources/Icon/lannode_online.png", UriKind.Absolute);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
