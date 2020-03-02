using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using XCore;
using XCore.Model;
using XCore.UserControl;

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
           
            NodeListBox.ItemsSource = Client.Nodes;
            DataContext = this;
        }

        private void Btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            var url = ((XCore.UserControl.MyImageButton)sender).Tag;
            if (url != null)
            {
                if (url.ToString().StartsWith("http"))
                {
                    Common.OpenUrl(url.ToString());
                   
                }
                else {
                    Common.OpenUrl("http://" + url.ToString() + ":8080");
               
                }
            }

        }
        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            var id = ((XCore.UserControl.MyImageButton)sender).Tag;
            if (id != null)
            {
                var msg = new MyMessageBox(MessageBoxButton.YesNo) { MessageText = "您确认要删除该节点吗？" };
                Common.ShowMessageControl(msg);

                if (msg.Result == MessageBoxResult.Yes)
                {
                    Client.StartAction(() =>
                    {

                        Common.ShowToast("正在删除节点...");
                        var response = Client.DeleteNode(Convert.ToInt32(id));
                        if (response.Result)
                        {
                            if (response.Json["errcode"].ToString() == "0")
                            {
                                Common.ShowToast(response.Json["msg"].ToString(), Color.FromRgb(0, 188, 0));
                                Common.UiAction(() => { Client.RemoveNode(Convert.ToInt32(id)); });
                            }
                        }
                        else { Common.ShowToast(response.Message, Color.FromRgb(239, 34, 7)); }
                       


                    });
                }
            }

        }
        private void Btn_AddToCloud_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Name.Text = "";
            TextBox_Serial.Text = "";
            TextBox_Serial.IsEnabled = true;
            if (e.OriginalSource.GetType().Name.Contains("MyImageButton"))
            {
                var Serial = ((XCore.UserControl.MyImageButton)sender).Tag;
                if (Serial != null)
                {

                    var node = Client.Nodes.ToList().Find(p => p.Serial == Serial.ToString());
                    if (node != null)
                    {
                        TextBox_Name.Text = node.Name;
                        TextBox_Serial.Text = node.Serial;
                    }
                }
            }
            MyMessageBox msg = new MyMessageBox(MessageBoxButton.YesNo, text_yes: "绑定到当前授权", text_no: "取消") { MessageText = "" };
            ((ContentControl)((Border)msg.Content).FindName("ContentControl")).Content = ContentControl_AddToCloud.Content;
            Common.ShowMessageControl(msg);
            if (msg.Result == MessageBoxResult.Yes)
            {
                string newnode_Serial = TextBox_Serial.Text;
                string newnode_Name = TextBox_Name.Text;
                Client.StartAction(() =>
                {

                    Common.ShowToast("正在绑定节点到当前授权上...");
                    var response = Client.AddNode(newnode_Serial, newnode_Name);
                    if (response.Result)
                    {
                        if (response.Json["errcode"].ToString() == "0")
                        {
                           
                                Common.ShowToast(response.Json["msg"].ToString(), Color.FromRgb(0, 188, 0));
                        }
                        else
                        {
                            Common.ShowToast(response.Json["msg"].ToString(), Color.FromRgb(239, 34, 7));
                        }
                    }
                    else
                    {
                        Common.ShowToast(response.Message, Color.FromRgb(239, 34, 7));
                    }



                });
            }




        }
        private void Btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Name.Text = "";
            TextBox_Serial.Text = "";
            TextBox_Serial.IsEnabled = false;
           
                var Serial = ((XCore.UserControl.MyImageButton)sender).Tag;
            if (Serial != null)
            {

                var node = Client.Nodes.ToList().Find(p => p.Serial == Serial.ToString());
                if (node != null)
                {
                    TextBox_Name.Text = node.Name;
                    TextBox_Serial.Text = node.Serial;

                }


                MyMessageBox msg = new MyMessageBox(MessageBoxButton.YesNo, text_yes: "保存", text_no: "取消") { MessageText = "" };
                ((ContentControl)((Border)msg.Content).FindName("ContentControl")).Content = ContentControl_AddToCloud.Content;
                Common.ShowMessageControl(msg);
                if (msg.Result == MessageBoxResult.Yes)
                {
                    string newnode_Serial = TextBox_Serial.Text;
                    string newnode_Name = TextBox_Name.Text;
                    Client.StartAction(() =>
                    {

                        Common.ShowToast("正在修改节点名称...");
                        var response = Client.SetNode(node.Id, newnode_Name);
                        if (response.Result)
                        {
                            if (response.Json["errcode"].ToString() == "0")
                            {
                                node.Name = newnode_Name;
                                Common.ShowToast(response.Json["msg"].ToString(),Color.FromRgb(0,188,0));
                            }
                            else
                            {
                                Common.ShowToast(response.Json["msg"].ToString(), Color.FromRgb(239, 34, 7));
                            }
                        }
                        else
                        {
                            Common.ShowToast(response.Message, Color.FromRgb(239, 34, 7));
                        }



                    });
                }

            }


        }

        private void NodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NodeListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }
    }

   
}
