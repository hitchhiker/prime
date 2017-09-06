using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Prime.Utility
{
    public static partial class StringExtensions1
    {
        /// <summary>
        /// Simple function to remove email addresses from any text, replaces with given string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveEmailsLeniant(this string text, string replacement)
        {
            if (!text.Contains("@"))
                return text;
            const string patternLenient = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            return Regex.Replace(text, patternLenient, replacement, RegexOptions.IgnoreCase | RegexOptions.Compiled).Trim();
        }
        /// <summary>
        /// Remove any text in the string that contains http:// or https://
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveHttps(this string text)
        {
            return Regex.Replace(text,
                                 @"(?<l>^|(\<br\s{0,1}/\>)|(\<br\>)|(\</{0,1}p\s{0,1}\>)|[^>^\x22^=^'^`^\[^\|^:])(?<link>http(?:s){0,1}://[^\.]+\.+[^\s^\n^\r^\]^\x22^'^`^\[^\|^:^\<^\>]*)",
                                 "",
                                 RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        /// <summary>
        /// Removes any characters that cant be used on either a filename or web url. This only works for filenames, not paths.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SafeFilenameForFSAndUrl(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return "";
            var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
            return Regex.Replace(Regex.Replace(text, @"(?<remove>[^0-9a-z\-\.])", "-", options), @"(?<remove>(\-{2,}))", "-", options);
        }

        private static readonly Regex Commonwordsregex = new Regex(@"(?<remove>(?:-)+(only|something|somethings|want|wants|what|whats|think|thinks|need|needs|please|thanks|thanks|help|this|that|thats|well|very|much|some|time|here|heres|just|like|find|finds|long|make|makes|more|only|over|such|take|takes|kind|than|then|them|that|thats|your|from|know|knows|been|good|when|whens|were|many|they|come|have|with|will|which|their|theirs|there|theres|before|people|peoples|found|should|shall|every|today|todays|comment|comments|where|wheres|called|dont)(?=-)+)",multiLineOptions);

        private static readonly RegexOptions multiLineOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        public static string RemoveStopWords(this string text)
        {
            return String.IsNullOrEmpty(text) ? text : Commonwordsregex.Replace(text, "");
        }

        /// <summary>
        /// Generate a clean random string for things like image verification. 
        /// Doesn't generates vowels and visually confusing numbers / characters to help keep things simple.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>String</returns>
        public static string RandomEasyReadString(int length)
        {
            if (length == 1)
                return "r";
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
            string result = "r" + RandomText.Generate(length-1);
            result = Regex.Replace(result, @"[AEIFL1]", "t", options);
            result = Regex.Replace(result, @"[OU0L6J]", "x", options);
            return result;
        }

        public static string RandomFastString(int length)
        {
            if (length == 1)
                return "r";
            return "r" + RandomText.Generate(length - 1);
        }

        [Obsolete("Broken due to 'signing' and other reasons.")]
        public static string DecimalToVisualString(this long iDec)
        {
            try
            {
                if (iDec == 0 || iDec > ((Int64.MaxValue)/4096))
                    return "r";
                var vs = "";
                var result = new long[64];
                var maxBit = 64;
                for (; iDec > 0; iDec /= 17)
                {
                    var rem = iDec%17;
                    result[--maxBit] = rem;
                }
                for (var i = 0; i < result.Length; i++)
                    vs += ToChar(Convert.ToInt32(result.GetValue(i)) + 97);
                vs = vs.TrimStart(new[] {'a'});

                vs = vs.Replace("a", "r");
                vs = vs.Replace("e", "v");
                vs = vs.Replace("i", "w");
                vs = vs.Replace("o", "x");
                vs = vs.Replace("u", "y");
                vs = vs.Replace("f", "z");
                return vs;
            }
            catch
            {
                return "r";
            }
        }

        public static long VisualStringToDecimal(this string vs)
        {
            try
            {
                vs = vs.Replace("r", "a");
                vs = vs.Replace("v", "e");
                vs = vs.Replace("w", "i");
                vs = vs.Replace("x", "o");
                vs = vs.Replace("y", "u");
                vs = vs.Replace("z", "f");

                double i = 0;
                var pos = vs.Length - 1;
                foreach (var c in vs)
                {
                    i += ((Math.Pow(17, pos))*((ToAscii(c) - 97)));
                    pos--;
                }
                return Convert.ToInt64(i);
            }
            catch
            {
                return 0;
            }
        }

        private static readonly string _base28 = "023456789bcdghjkmnpqrstvwxyz";

        /// <summary>
        /// Encode the given number into a Base28 string
        /// http://www.stum.de/2008/10/20/base36-encoderdecoder-in-c/
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String Visual2(long input)
        {
            return new string(Base28(input));
        }
        public static String Visual2(ulong input)
        {
            return new string(Base28(input));
        }

        public static char[] Base28(this ulong input)
        {
            var clistarr = _base28.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(clistarr[input % 28]);
                input /= 28;
            }
            return result.ToArray();
        }

        public static char[] Base28(this long input)
        {
            if (input <= 0) throw new ArgumentOutOfRangeException(nameof(input), input, "input cannot be negative or 0");

            var clistarr = _base28.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(clistarr[input % 28]);
                input /= 28;
            }
            return result.ToArray();
        }

        /// <summary>
        /// Decode the Base28 Encoded string into a number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int64 Base28(this string input)
        {
            var reversed = input.ToLower().Reverse();
            long result = 0;
            var pos = 0;
            foreach (var c in reversed)
            {
                result += _base28.IndexOf(c) * (long)Math.Pow(28, pos);
                pos++;
            }
            return result;
        }
    }
}