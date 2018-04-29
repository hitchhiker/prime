using System;

namespace Prime.Core
{
    public static class TimeExtensionMethods
    {
        public static DateTime ConformToResolution(this DateTime time, TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, DateTimeKind.Utc);
                case TimeResolution.Hour:
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0, DateTimeKind.Utc);
                case TimeResolution.Minute:
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, DateTimeKind.Utc);
                default:
                    return time;
            }
        }    
    }
}