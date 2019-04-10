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
using Xky.Core;

namespace Xky.Platform.UserControl.Pages
{
    /// <summary>
    /// MyNode.xaml 的交互逻辑
    /// </summary>
    public partial class MyNode 
    {
        public MyNode()
        {
            InitializeComponent();
            NodeListBox.ItemsSource = Client.Nodes;
        }

        private void NodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NodeListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }
    }
}
