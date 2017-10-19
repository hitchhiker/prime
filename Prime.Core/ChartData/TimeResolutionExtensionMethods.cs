using System;
using System.Collections.Generic;
using NodaTime;
using Prime.Utility;

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
                    return TimeSpan.FromHours(4);

                case TimeResolution.Hour:
                    return TimeSpan.FromDays(6);

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
            (TimeSpan.FromDays(30), TimeSpan.FromDays(365 * 10)),
            (TimeSpan.FromHours(30), TimeSpan.FromDays(12)),
            (TimeSpan.FromMinutes(30), TimeSpan.FromHours(6))
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

        public static DateTime Neighbour(this TimeResolution timeResolution, DateTime current, int distance  =1)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return current.AddDays(distance);
                case TimeResolution.Hour:
                    return current.AddHours(distance);
                case TimeResolution.Minute:
                    return current.AddMinutes(distance);
            }
            return DateTime.MinValue;
        }

        public static DateTime AddUnits(this DateTime current, TimeResolution timeResolution, int units)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return current.AddDays(units);
                case TimeResolution.Hour:
                    return current.AddHours(units);
                case TimeResolution.Minute:
                    return current.AddMinutes(units);
            }
            return current;
        }

        /// <summary>
        /// This method determines how much variation in the client clock is reasonable to allow for each resolution.
        /// </summary>
        /// <param name="timeResolution"></param>
        /// <returns></returns>
        public static int LiveTolerance(this TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Day:
                    return 2;
                case TimeResolution.Hour:
                    return 10;
                case TimeResolution.Minute:
                    return 30;
            }
            return 1;
        }

        public static bool IsLive(this DateTime time, TimeResolution timeResolution)
        {
            if (time.IsBeforeTheLast(TimeSpan.FromHours(30)))
                return false;

            return time >= timeResolution.LiveStartsAt();
        }

        public static DateTime LiveStartsAt(this TimeResolution timeResolution)
        {
            switch (timeResolution)
            {
                case TimeResolution.Minute:
                    return DateTime.UtcNow.AddMinutes(-2);

                case TimeResolution.Hour:
                    return DateTime.UtcNow.AddMinutes(-90);

                case TimeResolution.Day:
                    return DateTime.UtcNow.AddHours(-30);

                default:
                    throw new ArgumentOutOfRangeException(nameof(GetDefaultTimeSpan) + " in " + typeof(TimeResolutionExtensionMethods));
            }
        }
    }
}