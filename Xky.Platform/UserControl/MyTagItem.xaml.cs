using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Xky.Platform.UserControl
{
    /// <summary>
    ///     MyTagItem.xaml 的交互逻辑
    /// </summary>
    public partial class MyTagItem : System.Windows.Controls.UserControl
    {
        //用于判断选中状态
        public static readonly List<MyTagItem> ItemList = new List<MyTagItem>();

        public static readonly DependencyProperty TagIsSelectedProperty =
            DependencyProperty.Register("TagIsSelected", typeof(bool), typeof(MyTagItem),
                new PropertyMetadata(false, null));

        public MyTagItem()
        {
            InitializeComponent();
            DataContext = this;
            ItemList.Add(this);
        }


        public bool TagIsSelected
        {
            get => (bool) GetValue(TagIsSelectedProperty);
            set => SetValue(TagIsSelectedProperty, value);
        }

        internal event OnClick OnClickEvent;

        public void ClickDown(object sender, MouseButtonEventArgs e)
        {
            if (e == null || e.LeftButton == MouseButtonState.Pressed)
            {
                foreach (var myTagItem in ItemList) myTagItem.TagIsSelected = Equals(myTagItem, this);

                if (TagIsSelected) OnClickEvent?.Invoke(this);
            }
        }

        internal delegate void OnClick(MyTagItem sender);
    }
}