using System.Windows;
using System.Windows.Controls;
using Xky.UI.Controls.Other;
using Xky.UI.Data;
using Xky.UI.Tools.Helper;

namespace Xky.UI.Tools.StyleSelector
{
    public class RadioGroupItemStyleSelector : System.Windows.Controls.StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (container is RadioGroup radioGroup && item is UIElement radioButton)
            {
                var count = radioGroup.Items.Count;
                if (count == 1)
                {
                    return ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemSingle);
                }

                var index = radioGroup.Items.IndexOf(radioButton);
                return radioGroup.Orientation == Orientation.Horizontal
                    ? index == 0
                        ? ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemHorizontalFirst)
                        : ResourceHelper.GetResource<Style>(index == count - 1
                            ? ResourceToken.RadioGroupItemHorizontalLast
                            : ResourceToken.RadioGroupItemDefault)
                    : index == 0
                        ? ResourceHelper.GetResource<Style>(ResourceToken.RadioGroupItemVerticalFirst)
                        : ResourceHelper.GetResource<Style>(index == count - 1
                            ? ResourceToken.RadioGroupItemVerticalLast
                            : ResourceToken.RadioGroupItemDefault);
            }

            return null;
        }
    }
}