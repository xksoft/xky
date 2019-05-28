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

namespace Xky.XModule.Demo
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public List<Core.XModule> xmodules;
        public ModulePanel()
        {
            InitializeComponent();
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < xmodules.Count; i++)
            {
                ((Demo)xmodules[i]).t = i;
            }
            Client.CloseDialogPanel();
        }

    }
}
