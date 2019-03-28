using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Xky.Platform.UserControl
{
    public class HoverColorConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = int.Parse( parameter.ToString());
            var color = ((SolidColorBrush) value).Color;
            color = Color.FromRgb((byte) (color.R + num), (byte) (color.G + num), (byte) (color.B + num));

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = int.Parse(parameter.ToString());
            var color = ((SolidColorBrush) value).Color;
            color = Color.FromRgb((byte) (color.R - num), (byte) (color.G - num), (byte) (color.B - num));

            return new SolidColorBrush(color);
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}