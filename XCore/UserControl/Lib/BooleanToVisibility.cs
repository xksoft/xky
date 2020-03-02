using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace XCore.UserControl.Lib
{
    public class BooleanToVisibility : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ivalue = value as bool?;

            if (ivalue.HasValue && ivalue.Value) return Visibility.Visible;

            if (parameter != null)
                return Visibility.Collapsed;
            return Visibility.Hidden;
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