#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Prime.Core;

#endregion

namespace Prime.Core
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

    }
}