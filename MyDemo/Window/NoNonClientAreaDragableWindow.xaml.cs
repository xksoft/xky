using System.Windows;

namespace MyDemo.Window
{
    public partial class NoNonClientAreaDragableWindow
    {
        public NoNonClientAreaDragableWindow()
        {
            InitializeComponent();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}
