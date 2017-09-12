using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace Prime.Utility
{
    public static class DateTimeExt
    {
        public static long TotalSeconds(this DateTime datetime)
        {
            return (long) TimeSpan.FromTicks(datetime.Ticks).TotalSeconds;
        }

        public static long JavascriptTicks(this DateTime utc)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)(utc - unixEpoch).TotalMilliseconds;
        }

        public static DateTime GetJavascriptTime(this long jsMilliseconds)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return unixEpoch.AddMilliseconds(jsMilliseconds);
        }

        public static DateTime Min(this DateTime dateTime, DateTime minimumDateTime)
        {
            if (minimumDateTime>dateTime)
                return minimumDateTime;
            return dateTime;
        }

        //http://stackoverflow.com/a/1379158
        /// <summary>
        /// Adds the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime AddBusinessDays(this DateTime current, int days)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                }
                while (current.DayOfWeek == DayOfWeek.Saturday ||
                    current.DayOfWeek == DayOfWeek.Sunday);
            }
            return current;
        }

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime SubtractBusinessDays(this DateTime current, int days)
        {
            return AddBusinessDays(current, -days);
        }

        public static DateTime GetThisOrNearestBusinessDay(this DateTime current)
        {
            switch (current.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return current.AddDays(-1);
                case DayOfWeek.Sunday:
                    return current.AddDays(1);
            }
            return current;
        }

        public static DateTime GetThisOrNextBusinessDay(this DateTime current)
        {
            switch (current.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return current.AddDays(2);
                case DayOfWeek.Sunday:
                    return current.AddDays(1);
            }
            return current;
        }

        public static DateTime GetThisOrPreviousBusinessDay(this DateTime current)
        {
            switch (current.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return current.AddDays(-1);
                case DayOfWeek.Sunday:
                    return current.AddDays(-2);
            }
            return current;
        }

        public static DateTime GetMostRecentDayOfWeek(this DateTime current, DayOfWeek dayOfWeek)
        {
            var diff = current.DayOfWeek - dayOfWeek;
            if (diff == 0)
                return current;
            return diff > 0 ? current.AddDays(-1 * diff) : current.AddDays(-1 * (7 + diff));
        }

        public static DateTime GetNextBusinessDay(this DateTime current)
        {
            switch (current.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return current.AddDays(3);
                case DayOfWeek.Saturday:
                    return current.AddDays(2);
            }
            return current.AddDays(1);
        }
        
        public static DateTime GetLastBusinessDayOfMonth(this DateTime current)
        {
            return GetThisOrPreviousBusinessDay(new DateTime(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month)));
        }

        public static DateTime GetFirstBusinessDayOfMonth(this DateTime current)
        {
            return GetThisOrNextBusinessDay(new DateTime(current.Year, current.Month, 1));
        }

        public static DateTime GetFirstBusinessDayOfYear(this DateTime current)
        {
            return GetThisOrNextBusinessDay(new DateTime(current.Year, 1, 1));
        }

        public static bool IsFresh(this DateTime dateTime, TimeSpan withinTimeSpan)
        {
            return dateTime >= DateTime.UtcNow.Add(-withinTimeSpan);
        }

        public static bool IsStale(this DateTime dateTime, TimeSpan withinTimeSpan)
        {
            return dateTime <= DateTime.UtcNow.Add(-withinTimeSpan);
        }

        /// <summary>
        /// https://stackoverflow.com/a/24906105/1318333
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(this double unixTimeStamp)
        {
            var unixTimeStampInTicks = (long)(unixTimeStamp * TimeSpan.TicksPerSecond);
            return new DateTime(UnixEpoch.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        /// <summary>
        /// https://stackoverflow.com/a/24906105/1318333
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ToUnixTimeStamp(this DateTime dateTime)
        {
            var unixTimeStampInTicks = (dateTime.ToUniversalTime() - UnixEpoch).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public static double ToUnixTimeStampSimple(this DateTime dateTime)
        {
            return (dateTime - UnixEpoch).TotalSeconds;
        }

        public static Instant ToInstantLocal(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(nameof(dateTime) + " must be of Kind " + DateTimeKind.Utc);

            var utcf = new DateTime(dateTime.ToLocalTime().Ticks, DateTimeKind.Utc);

            return Instant.FromDateTimeUtc(utcf);
        }

        public static Instant ToInstant(this DateTime dateTime)
        {
            return Instant.FromDateTimeUtc(dateTime);
        }

        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
