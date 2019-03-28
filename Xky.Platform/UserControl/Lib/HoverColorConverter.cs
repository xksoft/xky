using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Xky.Platform.UserControl.Lib
{
    public class HoverColorConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(parameter != null, nameof(parameter) + " != null");
            var num = int.Parse(parameter.ToString());
            var color = (value as SolidColorBrush).Color;

            //判断防止越界
            if (color.R + num >= 0 && color.R + num <= 255)
            {
                color.R = (byte)(color.R + num);
            }
            else if (color.R + num < 0)
            {
                color.R = 0;
            }
            else
            {
                color.R = 255;
            }

            if (color.G + num >= 0 && color.G + num <= 255)
            {
                color.G = (byte) (color.G + num);
            }
            else if (color.G + num < 0)
            {
                color.G = 0;
            }
            else
            {
                color.G = 255;
            }


            if (color.B + num >= 0 && color.B + num <= 255)
            {
                color.B = (byte)(color.B + num);
            }
            else if (color.B + num < 0)
            {
                color.B = 0;
            }
            else
            {
                color.B = 255;
            }

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}