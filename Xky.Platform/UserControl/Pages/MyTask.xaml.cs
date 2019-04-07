using System;
using System.Collections.Generic;

namespace Xky.Platform.UserControl.Pages
{
    /// <summary>
    ///     MyTask.xaml 的交互逻辑
    /// </summary>
    public partial class MyTask
    {
        public MyTask()
        {
            var R = new Random();
            InitializeComponent();
            var list = new List<XkyTask>();
            for (var i = 1; i < 9999; i++)
                list.Add(new XkyTask {Id = i, Name = "点赞加好友", State = "执行中", Process = R.Next(500, 9999)});


            datagrid_task.ItemsSource = list;
        }
    }

    public class XkyTask
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = "加好友并点赞";
        public string State { get; set; } = "执行中";
        public int Process { get; set; }
    }
}