using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Prime.Utility
{
    public static partial class StringExtensionMethods
    {
        /// <summary>
        /// Equivalent to javascript's escape() function.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToEscapeJavascript(this string str)
        {
            return Uri.EscapeDataString(str);
        }

        /// <summary>
        /// Equivalent to javascript's unescape() function.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUnEscapeJavascript(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return Uri.UnescapeDataString(str);
        }

        public static string Truncate(this string str, int length, bool elipsis = true)
        {
            return str.TruncateString(length, elipsis ? TruncateOptions.IncludeEllipsis : TruncateOptions.None);
        }

        public static string TruncateWord(this string str, int words, bool elipsis = true)
        {
            return str.TruncateWords(words, elipsis);
        }

        /// <summary>
        /// Converts a string of hex values to a byte array.
        /// eg: 'a2 03 e4 ab' or 'abedfe'
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return new byte[0];

            hex = hex.RemoveControlAndNonAsciiCharacters().Replace(" ", string.Empty);

            return Enumerable.Range(0, hex.Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                 .ToArray();
        }

        public static string RemoveControlAndNonAsciiCharacters(this string input)
        {
            return Regex.Replace(new string(input.Where(c => !char.IsControl(c)).ToArray()), @"[^\u0020-\u007E]", string.Empty, RegexOptions.Compiled);
        }

        /// <summary>
        /// Removes all non word, non numeric characters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveNonWord(this string input)
        {
            return NonWordRegex.Replace(input, "");
        }

        public static Regex NonWordRegex = new Regex(@"\W+", RegexOptions.Compiled);


        public static string ToSimpleUrl(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            return str.Replace("+", "++").Replace(" ", "+");
        }

        public static string FromSimpleUrl(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            return str.Replace("++", "__REPLACE__").Replace("+", " ").Replace("__REPLACE__", "+");
        }

        public static bool IsSimilarContains(this string source, string toCheck)
        {
            var ci = new CultureInfo("en-US").CompareInfo;
            var co = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            return ci.IndexOf(source, toCheck, co) != -1;
        }

        private static readonly Regex SplitWordsRegex = new Regex(@"\W+", RegexOptions.Compiled);

        public static string[] SplitWords(this string str, StringSplitOptions options = StringSplitOptions.None)
        {
            if (options == StringSplitOptions.None)
                return SplitWordsRegex.Split(str);
            return SplitWordsRegex.Split(str).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        public static string FormatPhoneNumber(this string str, string format)
        {
            try
            {
                var pls = str.StartsWith("+");
                str = pls ? str.Substring(1) : str;
                var n = long.Parse(str);
                return (pls ? "+" : "") + n.ToString(format);
            }
            catch
            {
                return str;
            }
        }

        public static MemoryStream ToStream(this string s, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return new MemoryStream(encoding.GetBytes(s ?? ""));
        }
    }
}