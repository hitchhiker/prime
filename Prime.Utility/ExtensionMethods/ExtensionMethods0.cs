#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Prime.Utility;

#endregion

namespace Prime.Utility
{
    public static partial class ExtensionMethods
    {
        public static int? GetInt(this Dictionary<string, string> dict, string key)
        {
            if (dict == null || dict.Count == 0)
                return null;
            var val = dict.ContainsKey(key) ? dict[key] : null;
            if (val == null)
                return null;
            if (val.IsNumeric())
                return int.Parse(val);
            return null;
        }

        public static long ToNumber(this string input, long defaultValue)
        {
            if (!input.IsNumeric())
                return defaultValue;
            long v;
            return long.TryParse(input, out v) ? v : defaultValue;
        }

        public static int ToNumber(this string input, int defaultValue)
        {
            if (!input.IsNumeric())
                return defaultValue;
            int v;
            return int.TryParse(input, out v) ? v : defaultValue;
        }

        public static double ToNumber(this string input, double defaultValue)
        {
            if (!input.IsNumeric())
                return defaultValue;
            double v;
            return double.TryParse(input, out v) ? v : defaultValue;
        }



        public static int[] ToIntArray(this string input)
        {
            return string.IsNullOrEmpty(input) ?
                                                   new int[0] :
                                                                  input.Split(',').Where(x => x.IsNumeric()).Select(int.Parse).ToArray();
        }
        
        public static string ReplaceFirstOccurrance(this string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;
            var loc = original.IndexOf(oldValue);
            return loc == -1 ? original : original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        public static byte[] CompressToBytes(this string target)
        {
            return CompressedUtf8String.Compress(target);
        }

        public static string DecompressToString(this byte[] bytes)
        {
            return CompressedUtf8String.Expand(bytes);
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
            return basedate.AddDays(target*7);
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
    }
}