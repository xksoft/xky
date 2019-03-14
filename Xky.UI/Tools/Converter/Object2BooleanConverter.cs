using System;
using System.Globalization;
using System.Windows.Data;

namespace Xky.UI.Tools.Converter
{
    public class Object2BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(value is null);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}