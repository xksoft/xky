using System.Windows;

namespace Xky.UI.Controls.Base
{
    public interface ISelectable
    {
        event RoutedEventHandler Selected;

        bool IsSelected { get; set; }
    }
}