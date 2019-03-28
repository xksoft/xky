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

namespace Xky.Platform.UserControl.Pages
{
    /// <summary>
    /// MyTask.xaml 的交互逻辑
    /// </summary>
    public partial class MyTask 
    {
        public MyTask()
        {
            InitializeComponent();
            List<XkyTask> list = new List<XkyTask>();
            list.Add(new XkyTask { Id=1,Name="点赞加好友",State= "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });
            list.Add(new XkyTask { Id = 1, Name = "点赞加好友", State = "执行中" });

            datagrid_task.ItemsSource = list;
        }
    }
    public class XkyTask {
        private int _id =1;
        private string _name = "加好友并点赞";
        private string _state = "执行中";

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string State { get => _state; set => _state = value; }
    }
}
