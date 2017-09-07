using System;
using System.Collections.Generic;
using NodaTime;

namespace Prime.Core
{
    public static class TimeResolutionExtensionMethods {

        public static double GetAxisModifier(this TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Second:
                    return NodaConstants.TicksPerSecond;

                case TimeResolution.Minute:
                    return NodaConstants.TicksPerMinute;

                case TimeResolution.Hour:
                    return NodaConstants.TicksPerHour;

                case TimeResolution.Day:
                    return NodaConstants.TicksPerDay;

                case TimeResolution.Millisecond:
                    return NodaConstants.MillisecondsPerDay;

                default:
                    throw new ArgumentOutOfRangeException(nameof(GetAxisModifier) + " in " + typeof(TimeResolutionExtensionMethods));
            }
        }

        public static TimeSpan GetDefaultTimeSpan(this TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Second:
                    return TimeSpan.FromHours(1);

                case TimeResolution.Minute:
                    return TimeSpan.FromDays(1);

                case TimeResolution.Hour:
                    return TimeSpan.FromDays(3);

                case TimeResolution.Day:
                    return TimeSpan.FromDays(120);

                case TimeResolution.Millisecond:
                    return TimeSpan.FromSeconds(1);

                default:
                    throw new ArgumentOutOfRangeException(nameof(GetDefaultTimeSpan) + " in " + typeof(TimeResolutionExtensionMethods));
            }
        }

        private static readonly List<(TimeSpan, TimeSpan)> Timespans = new List<(TimeSpan, TimeSpan)>
        {
            (TimeSpan.FromDays(15), TimeSpan.FromDays(365 * 10)),
            (TimeSpan.FromHours(20), TimeSpan.FromDays(35)),
            (TimeSpan.FromMinutes(15), TimeSpan.FromDays(1))
        };

        public static TimeSpan MinTimeSpanRange(this TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return Timespans[0].Item1;
                case TimeResolution.Hour:
                    return Timespans[1].Item1;
                case TimeResolution.Minute:
                    return Timespans[2].Item1;
                default:
                    return TimeSpan.MinValue;
            }
        }

        public static TimeSpan MaxTimeSpanRange(this TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return Timespans[0].Item2;
                case TimeResolution.Hour:
                    return Timespans[1].Item2;
                case TimeResolution.Minute:
                    return Timespans[2].Item2;
                default:
                    return TimeSpan.MinValue;
            }
        }

        public static bool IsSmallerThan(this TimeResolution timeResolution, TimeResolution newResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return false;
                case TimeResolution.Hour:
                    return newResolution == TimeResolution.Day;
                case TimeResolution.Minute:
                    return newResolution == TimeResolution.Day || newResolution == TimeResolution.Hour;
                default:
                    return true;
            }
        }
    }
}