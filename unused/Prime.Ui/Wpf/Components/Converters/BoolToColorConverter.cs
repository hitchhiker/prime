using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Prime.Ui.Wpf.Components.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush Transparent = new SolidColorBrush(Colors.Transparent);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = parameter is SolidColorBrush ? (parameter as SolidColorBrush) : new SolidColorBrush(Colors.Gray);

            if (value == null)
                return Transparent;

            return System.Convert.ToBoolean(value) ? color : Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
