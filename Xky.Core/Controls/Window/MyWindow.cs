using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Xky.Core.Controls.Window
{
    public class MyWindow : System.Windows.Window
    {
        #region 窗口事件函数

        /// <summary>
        ///     窗口移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        /// <summary>
        ///     窗口最大化与还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void btn_max_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal; //设置窗口还原
            else
                WindowState = WindowState.Maximized; //设置窗口最大化
        }

        /// <summary>
        ///     窗口最小化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized; //设置窗口最小化
        }

        /// <summary>
        ///     关闭窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close(); //关闭窗口
        }

        private int i;

        /// <summary>
        ///     双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void dpTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //如果是右键点击，直接返回
            if (e.RightButton == MouseButtonState.Pressed)
                return;

            i += 1;
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) =>
            {
                timer.IsEnabled = false;
                i = 0;
            };
            timer.IsEnabled = true;

            //如果不是双击，直接返回
            if (i % 2 != 0)
                return;

            timer.IsEnabled = false;
            i = 0;
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion 窗口事件函数
    }
}