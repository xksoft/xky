﻿using System;
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
using XCore;

namespace Xky.XModule.ContactManager
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public List<XCore.XModule> xmodules;
        public List<string> list = new List<string>();
        public string username = "";
        public ModulePanel()
        {
            InitializeComponent();
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < xmodules.Count; i++)
            {
                //(ContactManager)xmodules[i]).t = i;
            }
            list = new List<string>() { "aaaa","bbbbbb"};
            username = "aaaaaaaaaaaaa";
            Client.CloseDialogPanel();
        }

    }
}
