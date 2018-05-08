using System;
using System.Windows.Data;
using Humanizer;

namespace Prime.Ui.Wpf.Components.Converters
{
    public class DateTimeHumanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DateTime dt))
                return string.Empty;

            return dt == DateTime.MinValue ? string.Empty : dt.Humanize();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}