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
            List<int[]> list = new List<int[]>();
            list.Add(new int[] { 1, 2, 3 });
            list.Add(new int[] { 2, 3, 4});
            list.Add(new int[] { 3, 4, 5 });

            int _col = list[0].Length;
            int _row = list.Count;
            for (int i = 0; i < _col; i++)
            {
                //datagrid_task.Columns.Add(new DataGridTextColumn
                //{
                //    Width = (Width - 30) / _col,
                //    Header = $"{(char)(65 + i)}",
                //    Binding = new Binding($"[{i.ToString()}]")
                //});
            }
            datagrid_task.ItemsSource = list;
        }
    }
}
