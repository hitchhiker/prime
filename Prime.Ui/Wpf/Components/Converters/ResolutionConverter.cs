using System;
using System.Globalization;
using System.Windows.Data;
using LiveCharts.Helpers;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    [ValueConversion(typeof(TimeResolution), typeof(PeriodUnit))]
    public class ResolutionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (targetType != typeof(PeriodUnit))
                throw new InvalidOperationException("The target must be a SeriesTimeResolution");

            switch ((TimeResolution)value)
            {
                case TimeResolution.Second:
                    return PeriodUnit.Seconds;

                case TimeResolution.Minute:
                    return PeriodUnit.Minutes;

                case TimeResolution.Hour:
                    return PeriodUnit.Hours;

                case TimeResolution.Day:
                    return PeriodUnit.Days;

                case TimeResolution.Millisecond:
                    return PeriodUnit.Milliseconds;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (targetType != typeof(TimeResolution))
                throw new InvalidOperationException("The target must be a TimeResolution");

            switch ((PeriodUnit)value)
            {
                case PeriodUnit.Seconds:
                    return TimeResolution.Second;

                case PeriodUnit.Minutes:
                    return TimeResolution.Minute;

                case PeriodUnit.Hours:
                    return TimeResolution.Hour;

                case PeriodUnit.Days:
                    return TimeResolution.Day;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}