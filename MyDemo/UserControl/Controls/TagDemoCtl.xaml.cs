using System;
using Xky.UI.Controls;

namespace MyDemo.UserControl
{
    public partial class TagDemoCtl
    {
        public TagDemoCtl()
        {
            InitializeComponent();
        }

        private void TagPanel_OnAddTagButtonClick(object sender, EventArgs e)
        {
            if (sender is TagPanel panel)
            {
                panel.Children.Add(new Tag
                {
                    Content = Properties.Langs.Lang.SubTitle
                });
            }
        }
    }
}
