using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
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

        public static bool IsWithinTheLast(this DateTime dateTime, TimeSpan withinTimeSpan)
        {
            return dateTime.IsAfterOrEqualTo(DateTime.UtcNow.Add(-withinTimeSpan));
        }

        public static bool IsBeforeTheLast(this DateTime dateTime, TimeSpan withinTimeSpan)
        {
            return dateTime.IsBeforeOrEqualTo(DateTime.UtcNow.Add(-withinTimeSpan));
        }

        public static bool IsAfterOrEqualTo(this DateTime dateTime, DateTime limit)
        {
            if (dateTime == DateTime.MinValue)
                return false;

            if (dateTime == DateTime.MaxValue)
                return true;

            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(nameof(IsWithinTheLast) + " only accepts UTC DateTime");

            return dateTime >= limit; //yes i realise these are kind of pointless.
        }

        public static bool IsBeforeOrEqualTo(this DateTime dateTime, DateTime limit)
        {
            if (dateTime == DateTime.MinValue)
                return true;

            if (dateTime == DateTime.MaxValue)
                return false;

            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(nameof(IsBeforeOrEqualTo) + " only accepts UTC DateTime");

            return dateTime <= limit; //yes i realise these are kind of pointless.
        }

        /// <summary>
        /// https://stackoverflow.com/a/24906105/1318333
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(this double unixTimeStamp)
        {
            var unixTimeStampInTicks = (long)(unixTimeStamp * TimeSpan.TicksPerSecond);
            return new DateTime(UnixEpoch.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
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

        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string ToPrettyString(this TimeSpan span)
        {
            if (span.Days > 0)
                return String.Format("{0}d:{1}h:{2}m", span.Days, span.Hours, span.Minutes);
            return span.Hours > 0 ? String.Format("{0}h:{1}m", span.Hours, span.Minutes) : String.Format("{0}m", span.Minutes);
        }

        public static string ToElapsed(this Stopwatch stopwatch, bool seconds = true)
        {
            var ts = stopwatch.Elapsed;
            if (seconds)
                return $"{ts.Seconds:0}.{ts.Milliseconds / 10:00}s";
            return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        }
        public static long SecondsLimited(this DateTime target, DateTime basedate)
        {
            return (long)(target - basedate).TotalSeconds;
        }

        public static long MinutesLimited(this DateTime target, DateTime basedate)
        {
            return (long)(target - basedate).TotalMinutes;
        }

        public static long HoursLimited(this DateTime target, DateTime basedate)
        {
            return (long)(target - basedate).TotalHours;
        }

        public static long DaysLimited(this DateTime target, DateTime basedate)
        {
            return (long)(target - basedate).TotalDays;
        }

        public static long WeeksLimited(this DateTime target, DateTime basedate)
        {
            return (long)(target - basedate).TotalDays / 7;
        }

        public static DateTime SecondsLimited(this long target, DateTime basedate)
        {
            return basedate.AddSeconds(target);
        }

        public static DateTime MinutesLimited(this long target, DateTime basedate)
        {
            return basedate.AddMinutes(target);
        }

        public static DateTime HoursLimited(this long target, DateTime basedate)
        {
            return basedate.AddHours(target);
        }

        public static DateTime DaysLimited(this long target, DateTime basedate)
        {
            return basedate.AddDays(target);
        }

        public static DateTime WeeksLimited(this long target, DateTime basedate)
        {
            return basedate.AddDays(target * 7);
        }

        public static DateTime SqlDateTimeMinValue = new DateTime(552877920000000000);

        public static DateTime SqlSafe(this DateTime input)
        {
            return input < SqlDateTimeMinValue ? SqlDateTimeMinValue : input;
        }

        /// <summary>
        /// Will match created/modified directory fimestamps recurssively, as long as the names match.
        /// </summary>
        public static void SyncTimeStamps(this DirectoryInfo target, DirectoryInfo source, bool recurse = true, bool ignoreErrors = false)
        {
            if (source.Name != target.Name || (source.LastWriteTimeUtc == target.LastWriteTimeUtc && source.CreationTimeUtc == target.CreationTimeUtc))
                return;

            if (ignoreErrors)
                try
                {
                    target.LastWriteTimeUtc = source.LastWriteTimeUtc;
                    target.CreationTimeUtc = source.CreationTimeUtc;
                }
                catch { return; }
            else
            {
                target.LastWriteTimeUtc = source.LastWriteTimeUtc;
                target.CreationTimeUtc = source.CreationTimeUtc;
            }

            if (recurse)
                SyncTimeStamps(source.Parent, target.Parent, true, ignoreErrors);
        }

        public static DateTime ChangedUtc(this FileInfo fileInfo)
        {
            return fileInfo.LastWriteTimeUtc > fileInfo.CreationTimeUtc ? fileInfo.LastWriteTimeUtc : fileInfo.CreationTimeUtc;
        }
    }
}
