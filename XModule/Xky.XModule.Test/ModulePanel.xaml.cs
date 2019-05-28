using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;
using Xky.Core.UserControl;

namespace Xky.XModule.Test
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public ModulePanel()
        {
            InitializeComponent();
        }
        public List<Test> xmodules;
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {

            for (int i=0;i<xmodules.Count;i++)
            {
                xmodules[i].t = i;
            }
            Client.CloseDialogPanel();
        }
       
       

      
    }
}
