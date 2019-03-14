using System.Collections.Specialized;
using System.Windows;
using Xky.UI.Controls.Base;

namespace Xky.UI.Controls.Transfer
{
    public class TransferGroup : SimpleItemsControl
    {
        protected override void Refresh()
        {
            if (Items.Count > 0)
            {
                foreach (FrameworkElement item in Items)
                {
                    item.Style = ItemContainerStyle;
                    ItemsHost.Children.Add(item);
                }
            }
        }

        protected override void OnItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        protected override void OnItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        protected override DependencyObject GetContainerForItemOverride() => new TransferItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TransferItem;

        protected override void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsHost == null) return;
            if (e.OldItems != null)
            {
                foreach (FrameworkElement item in e.OldItems)
                {
                    ItemsHost.Children.Remove(item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (FrameworkElement item in e.NewItems)
                {
                    item.Style = ItemContainerStyle;
                    ItemsHost.Children.Add(item);
                }
            }
        }
    }
}