using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Prime.Ui.Wpf
{
    [ValueConversion(typeof(bool), typeof(double))]
    public sealed class BoolToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? double.Parse(parameter?.ToString() ?? "0") : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception();
        }
    }
}