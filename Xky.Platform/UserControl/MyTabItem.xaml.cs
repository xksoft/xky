using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                foreach (var myTabItem in ItemList)
                {
                    myTabItem.IsSelected = Equals(myTabItem, this);
                    MyIcon.Margin = IsSelected ? new Thickness(-8, 0, 0, 0) : new Thickness(0);
                }
                if (IsSelected)
                {
                    OnClickEvent?.Invoke(this, PageName, IsDarkStyle);
                }
            }
        }

        private void MyTabItem_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MyIcon.Margin = IsSelected ? new Thickness(-8, 0, 0, 0) : new Thickness(0);
        }

        private void MyTabItem_OnMouseLeave(object sender, MouseEventArgs e)
        {
            MyIcon.Margin = new Thickness(-8, 0, 0, 0);
        }

        #endregion
    }
}