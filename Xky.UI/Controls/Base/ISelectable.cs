using System.Windows;

namespace Xky.UI.Controls
{
    public interface ISelectable
    {
        event RoutedEventHandler Selected;

        bool IsSelected { get; set; }
    }
}