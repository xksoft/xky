using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Xky.UI.Controls;
using Xky.UI.Data;
using Xky.UI.Tools;
using MyDemo.Data;
using MyDemo.Tools;

namespace MyDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GlobalData.Init();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(GlobalData.Config.Lang);

            if (GlobalData.Config.Skin != SkinType.Default)
            {
                UpdateSkin(GlobalData.Config.Skin);
            }

            BlurWindow.SystemVersionInfo = CommonHelper.GetSystemVersionInfo();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            GlobalData.Save();
        }

        internal void UpdateSkin(SkinType skin)
        {
            var skins0 = Resources.MergedDictionaries[0];
            skins0.MergedDictionaries.Clear();
            skins0.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            skins0.MergedDictionaries.Add(ResourceHelper.GetSkin(typeof(App).Assembly, "Resources/Themes", skin));

            var skins1 = Resources.MergedDictionaries[1];
            skins1.MergedDictionaries.Clear();
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Xky.UI;component/Themes/Theme.xaml")
            });
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MyDemo;component/Resources/Themes/Theme.xaml")
            });
            Current.MainWindow?.OnApplyTemplate();
        }
    }
}
