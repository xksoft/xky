using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            //ObservableCollection<Node> Nodes = new ObservableCollection<Node>();
            //foreach (Node n in Client.Nodes)
            //{
            //    Nodes.Add(n);

            //}
            //foreach (var n in Client.LocalNodes)
            //{
            //    Nodes.Add(n.Value);

            //}
            //NodeListBox.ItemsSource = Nodes;
            NodeListBox.ItemsSource = Client.LocalNodes;
            NodeListBox.SourceUpdated += NodeListBox_SourceUpdated;
        }

        private void NodeListBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            Console.WriteLine("ccccccccccccccccccccccccc");
        }

        private void NodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NodeListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }
    }
}
