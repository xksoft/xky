using System;
using System.IO;
using System.Net;
using System.Windows;

namespace Xky.Platform
{
    /// <summary>
    ///     App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (!Directory.Exists("x86") || !Directory.Exists("x64"))
            {
                MessageBox.Show("缺少ffmpeg库文件，请先下载解压到程序目录下！\r\n下载地址：https://static.xky.com/download/ffmbeg_libs.rar",
                    "缺少ffmepg库文件", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }

            //最多512个并发
            ServicePointManager.DefaultConnectionLimit = 512;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Console.WriteLine(ex.Message + ex.StackTrace);
        }
    }
}