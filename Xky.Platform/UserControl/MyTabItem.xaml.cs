using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Xky.Core;

namespace Xky.Platform.UserControl
{
    /// <summary>
    /// MyTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class MyTabItem
    {
        public MyTabItem()
        {
            InitializeComponent();
            ItemList.Add(this);
        }

        //用于判断选中状态
        private static readonly List<MyTabItem> ItemList = new List<MyTabItem>();

        #region  自定义事件

        internal event OnClick OnClickEvent;

        internal delegate void OnClick(MyTabItem sender, string pagename, bool dark);

        #endregion

        #region 自定义控件属性

        public string PageName
        {
            get => (string) GetValue(PageNameProperty);
            set => SetValue(PageNameProperty, value);
        }

        public static readonly DependencyProperty PageNameProperty =
            DependencyProperty.Register("PageName", typeof(string), typeof(MyTabItem),
                new PropertyMetadata(null, null));


        public bool IsDarkStyle
        {
            get => (bool) GetValue(IsDarkStyleProperty);
            set => SetValue(IsDarkStyleProperty, value);
        }

        public static readonly DependencyProperty IsDarkStyleProperty =
            DependencyProperty.Register("IsDarkStyle", typeof(bool), typeof(MyTabItem),
                new PropertyMetadata(false, null));

        public bool CheckLicense
        {
            get => (bool) GetValue(CheckLicenseProperty);
            set => SetValue(CheckLicenseProperty, value);
        }

        public static readonly DependencyProperty CheckLicenseProperty =
            DependencyProperty.Register("CheckLicense", typeof(bool), typeof(MyTabItem),
                new PropertyMetadata(true, null));

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MyTabItem),
                new PropertyMetadata(false, null));

        public ImageSource ImgSource
        {
            get => (ImageSource) GetValue(ImgSourceProperty);
            set => SetValue(ImgSourceProperty, value);
        }

        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(MyTabItem),
                new PropertyMetadata(null, null));

        #endregion

        #region 事件

        public void ClickDown(object sender, MouseButtonEventArgs e)
        {
            if (e == null || e.LeftButton == MouseButtonState.Pressed)
            {
                if (CheckLicense && Client.License == null)
                {
                    Common.ShowToast("需要先授权才能执行本操作", Color.FromRgb(255,36,50),"off");
                    return;
                }


                foreach (var myTabItem in ItemList)
                {
                    myTabItem.IsSelected = Equals(myTabItem, this);
                    myTabItem.MyCanvas.Background = myTabItem.IsSelected
                        ? new SolidColorBrush(Color.FromRgb(46, 165, 255))
                        : new SolidColorBrush(Colors.Transparent);
                }

                if (IsSelected)
                {
                    OnClickEvent?.Invoke(this, PageName, IsDarkStyle);
                }
            }
        }

        #endregion
    }
}