using System.Windows.Controls;

namespace Xky.UI.Controls.Button
{
    /// <summary>
    ///     带上下文菜单的按钮
    /// </summary>
    public class ContextMenuButton : System.Windows.Controls.Button
    {
        public ContextMenu Menu { get; set; }

        protected override void OnClick()
        {
            base.OnClick();
            if (Menu != null)
            {
                Menu.PlacementTarget = this;
                Menu.IsOpen = true;
            }
        }
    }
}