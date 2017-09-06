#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Prime.Utility;

#endregion

namespace Prime.Utility
{
    [Flags]
    public enum TruncateOptions
    {
        None = 0x0,
        FinishWord = 0x1,
        AllowLastWordToGoOverMaxLength = 0x2,
        IncludeEllipsis = 0x4
    }

    public static partial class StringExtensions1
    {
        public const int ObShift = 119;
      
        public static string RemoveHashFromUrl(this string url)
        {
            return String.IsNullOrEmpty(url) ? url : (url.Contains("#") ? url.Substring(0, url.IndexOf("#")) : url);
        }

        public static string IpNumber2Address(this uint number)
        {
            var w = (number/16777216)%256;
            var x = (number/65536)%256;
            var y = (number/256)%256;
            var z = (number)%256;
            return w + "." + x + "." + y + "." + z;
        }

        public static string ValidateNumericCsv(this string csv)
        {
            if (String.IsNullOrEmpty(csv))
                return "";
            var fiFinal = csv.Split(',').Where(IsNumeric).ToList();
            return fiFinal.Count > 0 ? String.Join(",", fiFinal.ToArray()) : "";
        }

        public static string ValidateCountryCodeCsv(this string csv)
        {
            if (String.IsNullOrEmpty(csv))
                return "";
            var fiFinal =
                csv.Split(',').Select(x => x.Trim()).Where(y => y.Length < 3).Select(z => z.ToUpper()).ToList();
            return fiFinal.Count > 0 ? String.Join(",", fiFinal.ToArray()) : "";
        }
        
        public static string[] SplitByNewline(this string str)
        {
            return String.IsNullOrEmpty(str) ? new string[0] : str.Replace("\r", "").Split('\n');
        }
        

        public static string ConfirmDirTrailingSlash(this string path)
        {
            return ConfirmDirTrailingSlash(path, false);
        }

        public static string ConfirmDirTrailingSlash(this string path, bool UseSeperatorForNull)
        {
            if (!String.IsNullOrEmpty(path) && path.Length > 1)
            {
                if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                    path += Path.DirectorySeparatorChar;
                return path;
            }
            return UseSeperatorForNull ? Path.DirectorySeparatorChar.ToString() : null;
        }

        public static int CountOccurencesOfChar(this string instance, char c)
        {
            return instance.Count(curChar => c == curChar);
        }
        /// <summary>
        ///     used with usenet break method UsenetBreakText72
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <param name="trail"></param>
        /// <returns></returns>
        private static (string, string) SummariseReturnTrail(this string text, int length)
        {
            if (text == null)
                return ("", "");
            if (text.Length <= length || length < 5 || text.Length < 4)
                return (text, "");
            var lastSpace = text.LastIndexOf(' ', length - 3);
            if (lastSpace < 4)
                return (text.Substring(0, length - 3), text.Substring(length - 3));
            return (text.Substring(0, text.LastIndexOf(' ', length - 3)),
                                            text.Substring(text.LastIndexOf(' ', length - 3)));
        }
        
        public static String Summarise(this string text, int length, bool ellipsis, bool trim = true)
        {
            if (text == null)
                return "";

            if (trim)
                text = text.Trim();

            var opt = ellipsis ? TruncateOptions.FinishWord | TruncateOptions.IncludeEllipsis : TruncateOptions.FinishWord;
            return TruncateString(text, length, opt);
        }

        /// <summary>
        /// http://www.codeproject.com/Articles/47930/String-Truncation-Function-for-C
        /// </summary>
        /// <param name="valueToTruncate"></param>
        /// <param name="maxLength"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string TruncateString(this string valueToTruncate, int maxLength, TruncateOptions options = TruncateOptions.FinishWord | TruncateOptions.IncludeEllipsis)
        {
            if (valueToTruncate == null)
                return "";

            if (valueToTruncate.Length <= maxLength)
                return valueToTruncate;

            var includeEllipsis = (options & TruncateOptions.IncludeEllipsis) == TruncateOptions.IncludeEllipsis;
            var finishWord = (options & TruncateOptions.FinishWord) == TruncateOptions.FinishWord;
            var allowLastWordOverflow = (options & TruncateOptions.AllowLastWordToGoOverMaxLength) == TruncateOptions.AllowLastWordToGoOverMaxLength;

            var retValue = valueToTruncate;

            if (includeEllipsis)
                maxLength -= 1;
            

            var lastSpaceIndex = retValue.LastIndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);

            if (!finishWord)
                retValue = retValue.Remove(maxLength);
            else if (allowLastWordOverflow)
            {
                var spaceIndex = retValue.IndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);
                if (spaceIndex != -1)
                {
                    retValue = retValue.Remove(spaceIndex);
                }
            }
            else if (lastSpaceIndex > -1)
                retValue = retValue.Remove(lastSpaceIndex);
            
            if (includeEllipsis && retValue.Length < valueToTruncate.Length)
                retValue += "...";
            
            return retValue;
        }

        public static string TruncateWords(this string value, int maxLength, bool addElipsis = false, bool trim = true)
        {
            value = trim ? value?.Trim() : value;

            if (value == null || value.Trim().Length <= maxLength)
                return value;

            var ellipse = addElipsis ? "..." : string.Empty;

            var truncateChars = new char[] { ' ', ',', '.', '!', '?', ':', ';', '"', '\'' };
            var index = value.Trim().LastIndexOfAny(truncateChars);

            while ((index + ellipse.Length) > maxLength)
                index = value.Substring(0, index).Trim().LastIndexOfAny(truncateChars);

            if (index > 0)
                return value.Substring(0, index) + ellipse;

            return value.Substring(0, maxLength - ellipse.Length) + ellipse;
        }

        /// <summary>
        ///     Breaks an English text string into a IList of words
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> GetWords(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return new List<string>();

            const string regex = @"^(?:(?<word>[^\s\r\f\n\.\:\)\(\!\?\,]*)[\s\r\n\f\.\:\)\(\!\?\,]{1,})+";
            var matches = Regex.Matches(" " + text + " ", regex, RegexOptions.Compiled | RegexOptions.Singleline);
            if (matches.Count == 0)
                return new List<string>();
            if (!matches[0].Groups["word"].Success)
                return new List<string>();
            return
                (from Capture cap in matches[0].Groups["word"].Captures where cap.Length > 0 select cap.Value).Distinct()
                                                                                                              .ToList();
        }

        /// <summary>
        ///     This function is only an approximation. It will attempt to snip out extra words in the center part of the text.
        ///     It will allow a degree of flexibility, by setting the trigger you can determine the amount of extra words that will
        ///     trigger the function.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="words">Amount of words you'd like to aim for (Even number)</param>
        /// <param name="trigger">Amount of words over your specified level, that will trigger the routine</param>
        /// <returns></returns>
        public static String TruncateWordsCentered(this string text, int words, int trigger)
        {
            //first count the words
            var regex = @"^(?<word>[\S\r\n]* )*(?<lw>[\S]*)"; //cant work out this last word issue for now.. hacky time
            var result = Regex.Match(text, regex, RegexOptions.Compiled);

            var w = (from Capture c in result.Groups["word"].Captures select c.Value).ToList();
            w.Add(result.Groups["lw"].Value);

            if (w.Count <= words + trigger)
                return text;

            words = words/2;
            //catch from front, append ... catch from end
            var str = new StringBuilder();

            for (var i = 1; i <= words; i++)
                str.Append(w[i - 1]);

            str.Append(" ... ");

            for (var i = 1 + (w.Count - words); i <= w.Count; i++)
                str.Append(w[i - 1]);

            return str.ToString().Replace("  ", " ");
        }

        public static string Base64Encode(this string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;
            try
            {
                var encDataByte = Encoding.UTF8.GetBytes(data);
                var encodedData = Convert.ToBase64String(encDataByte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        public static string Base64Decode(this string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            try
            {
                var encoder = new UTF8Encoding();
                var utf8Decode = encoder.GetDecoder();

                var todecodeByte = Convert.FromBase64String(data);
                var charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                var decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                var result = new String(decodedChar);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
        
        public static string DeObfuscate(this string hexstring)
        {
            if (String.IsNullOrEmpty(hexstring))
                return "";
            try
            {
                var sb = new StringBuilder(hexstring.Length/2);
                for (var i = 0; i <= hexstring.Length - 1; i = i + 2)
                {
                    var pi = Int32.Parse(hexstring.Substring(i, 2), NumberStyles.HexNumber);
                    pi = pi ^ ObShift;
                    sb.Append((char) pi);
                }
                return sb.ToString();
            }
            catch
            {
                return "-1";
            }
        }

        public static string Obfuscate(this string sData)
        {
            if (string.IsNullOrEmpty(sData))
                return "";

            var charArray = (sData.ToCharArray());

            var sb = new StringBuilder(charArray.Length*2);
            for (var i = 0; i < charArray.Length; i++)
            {
                if ((sData.Length - (i + 1)) > 0)
                {
                    var temp = sData.Substring(i, 2);
                    switch (temp)
                    {
                        case @"\n":
                            AppendObfuscatedChar(sb, (char) 10);
                            break;
                        case @"\b":
                            AppendObfuscatedChar(sb, (char) 32);
                            break;
                        case @"\r":
                            AppendObfuscatedChar(sb, (char) 13);
                            break;
                        case @"\c":
                            AppendObfuscatedChar(sb, (char) 44);
                            break;
                        case @"\\":
                            AppendObfuscatedChar(sb, (char) 92);
                            break;
                        case @"\0":
                            AppendObfuscatedChar(sb, (char) 0);
                            break;
                        case @"\t":
                            AppendObfuscatedChar(sb, (char) 7);
                            break;
                        default:
                            AppendObfuscatedChar(sb, charArray[i]);
                            i--;
                            break;
                    }
                }
                else
                    AppendObfuscatedChar(sb, charArray[i]);
                i++;
            }
            return sb.ToString();
        }

        private static void AppendObfuscatedChar(this StringBuilder builder, char chr)
        {
            builder.AppendFormat("{0:X2}", (chr ^ ObShift));
        }

        public static string CamelBack(this string s, string delimiter)
        {
            var result = new StringBuilder(s.Length*2);
            foreach (var part in s.Split(delimiter.ToCharArray()))
            {
                result.Append(Left(part, 1).ToUpper());
                if (part.Length > 1)
                    result.Append(Mid(part, 1).ToLower());
            }
            return result.ToString();
        }

        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }

        public static bool IsAllUpper(this string input)
        {
            if (input == null)
                return false;

            foreach (var t in input)
                if (char.IsLetter(t) && !char.IsUpper(t))
                    return false;

            return true;
        }

        /// <summary>
        ///     Capitalise the string after each occurance of the delimiter
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string Capitalise(this string s, string delimiter = " ")
        {
            if (s == null)
                return "";
            var result = new StringBuilder(s.Length*4);
            foreach (var part in s.Split(delimiter.ToCharArray()))
            {
                result.Append(Left(part, 1).ToUpper());
                if (part.Length > 1)
                    result.Append(Mid(part, 1));
                result.Append(delimiter);
            }
            return result.ToString().Trim(delimiter.ToCharArray());
        }

        public static string CapitaliseFirstLetter(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }

        public static string Left(this string param, int length)
        {
            if (String.IsNullOrEmpty(param))
                return "";
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            if (!(param.Length < length))
                return param.Substring(0, length);
            //return the result of the operation
            return param;
        }

        public static string Left(this string param, int length, bool addTrail)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            if (length < 4)
                return param;
            if (!(param.Length < length))
                return param.Substring(0, length - 3) + "...";
            //return the result of the operation
            return param;
        }

        public static string Right(this string param, int length)
        {
            if (length >= param.Length)
                return param;
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            var result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(this string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            var result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(this string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            var result = param.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        public static string QuickCleanHtml(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.IndexOf("<") > 0)
                str.Replace("<", "&lt;");
            if (str.IndexOf(">") > 0)
                str.Replace(">", "&gt;");
            return str;
        }

        public static int ToAscii(this char ch)
        {
            return Convert.ToInt32(ch);
        }

        public static Char ToChar(this int i)
        {
            //Return the character of the given character value
            return Convert.ToChar(i);
        }

        /// <summary>
        ///     Formats numbers, negative numbers are formatted by the 'format' string. If the number is null or not-numeric, it will return a blank string.
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="format"></param>
        /// <returns>HTML Formatted numeric string</returns>
        public static string FormatNegativeNumber(this string format, object candidate)
        {
            var _candidate = candidate.ToString();
            if ((!IsNumeric(_candidate)))
                return String.Empty;
            var number = Convert.ToDecimal(_candidate);
            return number < 0 ? String.Format(format, _candidate) : _candidate;
        }

        /// <summary>
        ///     Returns a string depending on 'candidate' being positive or negative, or NAN if not a number.
        /// </summary>
        public static string PositiveNegative(this string positive, string negative, string NAN, object candidate)
        {
            var _candidate = candidate.ToString();
            if ((!IsNumeric(_candidate)))
                return NAN;
            var number = Convert.ToDecimal(_candidate);
            return number < 0 ? negative : positive;
        }

        /// <summary>
        ///     Returns a formatted string depending on 'candidate' being positive or negative, or NAN if neither (0, or NAN)
        /// </summary>
        public static string FormatPositiveNegative(string FormatPositive, string FormatNegative, string NAN,
                                                    object candidate)
        {
            var _candidate = candidate.ToString();
            if (!IsNumeric(_candidate))
                return NAN;
            var number = Convert.ToDecimal(_candidate);
            if (number == 0)
                return NAN;
            return number < 0 ? String.Format(FormatNegative, number) : String.Format(FormatPositive, number);
        }

        public static bool IsNumeric(this string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate))
                return false;
            return Decimal.TryParse(candidate, out decimal result);
        }

        public static string CurrencyFormat(this string candidate)
        {
            return IsNumeric(candidate) ? CurrencyFormat(Convert.ToDecimal(candidate)) : "N/A";
        }

        public static string CurrencyFormat(this decimal number)
        {
            return String.Format("{0:0,0.00}", Math.Round(number, 2));
        }

        public static string ArrayListToString(this ArrayList ar)
        {
            return ArrayListToString(ar, ',');
        }

        public static string ArrayListToString(this ArrayList ar, char delim)
        {
            return ArrayListToString(ar, delim.ToString());
        }

        public static string ArrayListToString(this ArrayList ar, string delim)
        {
            return ar.Cast<object>().Aggregate("", (current, o) => current + (o + delim));
        }

        public static string FormattedPercentFraction(this double? fraction)
        {
            if (fraction == Double.MinValue || fraction == null)
                return "";
            return FormattedPercentFraction(fraction ?? 0);
        }

        public static string FormattedPercentFraction(this double fraction)
        {
            return fraction == Double.MinValue ? "" : String.Format("{0:f}%", Math.Round(fraction*100, 2));
        }

        public static string NulledNumber(this double number)
        {
            return number == Double.MinValue ? "" : number.ToString();
        }

        public static string FormattedRatio(this double? ratio)
        {
            if (ratio == Double.MinValue || ratio == null)
                return "";
            return FormattedRatio(ratio ?? 0);
        }

        public static string FormattedRatio(this double ratio)
        {
            return ratio == Double.MinValue ? "" : String.Format("{0:f}", Math.Round(ratio, 2));
        }

        /// <summary>
        ///     Replaces any word characters that repeat more than twice with only two repetitions of that char.
        ///     eg: Hellloooooo!!!! would become Helloo!!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveRepeatedChars(this string input)
        {
            try
            {
                var r = new Regex(@"(\w)\1{2,}",
                                  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(input, "$1$1");
            }
            catch
            {
                return input;
            }
        }

        public static string HumanisePastDate(this DateTime utcDate)
        {
            return HumanisePastDate(utcDate, true);
        }

        public static string HumanisePastDate(this DateTime utcDate, bool includeSeconds)
        {
            var dateNow = DateTime.UtcNow;
            if (utcDate > DateTime.UtcNow)
                return "in future";

            var time = dateNow - utcDate;

            if (includeSeconds && time.Days < 1 && time.Hours < 1 && time.Minutes < 1)
                if (time.Seconds < 1)
                    return "now";
                else
                    return time.Seconds + " secs ago";

            if (time.Days < 1 && time.Hours < 1 && time.Minutes < 1)
                return "<1 min ago";

            if (time.Days < 1)
            {
                if (time.Hours < 1)
                {
                    if (time.Minutes < 10 && includeSeconds) //9 min 25 secs ago
                        return time.Minutes + " min " + time.Seconds + " sec ago";
                    return time.Minutes + " mins ago"; // 15 mins ago
                }
                if (time.Minutes != 0)
                    return time.Hours + " hr " + time.Minutes + " min ago";

                return time.Hours + " hrs ago";
            }

            // if it's yesterday, just show the date and time (yesterday 2:36pm GMT)
            if (time.Days == 1)
                return "yesterday " + utcDate.ToString("h:mm tt").ToLower() + " GMT";

            // if it's less than 6 days ago, show the days and time (2 days ago 2:36pm GMT)
            if (time.Days <= 6)
                return time.Days + " days ago " + utcDate.ToString("h:mm tt").ToLower() + " GMT";
            ;

            if (time.Days < 31)
                return (Math.Abs(time.Days)) + " days ago";
            if (utcDate.Year == DateTime.UtcNow.Year)
                return String.Format(utcDate.ToString("{0} MMMM"), AddOrdinal(utcDate.Day));
            return String.Format(utcDate.ToString("{0} MMM yyyy"), AddOrdinal(utcDate.Day));
        }

        public static string AddOrdinal(this int num)
        {
            switch (num%100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num%10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        /// <summary>
        ///     Encodes a string to be represented as a string literal. The format
        ///     is essentially a JSON string.
        ///     Example Output: Hello \"Rick\"!\r\nRock on
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeJsString(this string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return "";
            }
            int i;
            var len = s.Length;
            var sb = new StringBuilder(len + 4);
            string t;

            for (i = 0; i < len; i += 1)
            {
                var c = s[i];
                if ((c == '\\') || (c == '"') || (c == '>') || (c == '\''))
                {
                    sb.Append('\\');
                    sb.Append(c);
                }
                else if (c == '\b')
                    sb.Append("\\b");
                else if (c == '\t')
                    sb.Append("\\t");
                else if (c == '\n')
                    sb.Append("\\n");
                else if (c == '\f')
                    sb.Append("\\f");
                else if (c == '\r')
                    sb.Append("\\r");
                else
                {
                    if (c < ' ')
                    {
                        //t = "000" + Integer.toHexString(c); 
                        var tmp = new string(c, 1);
                        t = "000" + Int32.Parse(tmp, NumberStyles.HexNumber);
                        sb.Append("\\u" + t.Substring(t.Length - 4));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }

        public static string ReplaceLinebreaks(this string text, string replacement)
        {
            if (String.IsNullOrEmpty(text))
                return text;
            text = text.Replace("\r\n", replacement).Replace("\r\n", "");
            text = text.Replace("\r", replacement).Replace("\r", "");
            text = text.Replace("\n", replacement).Replace("\n", "");
            return text;
        }

        public static string ConvertToUnicode(this string text)
        {
            var iso = Encoding.GetEncoding("iso8859-1");
            var unicode = Encoding.UTF8;
            var isoBytes = iso.GetBytes(text);

            return unicode.GetString(isoBytes);
        }


        public static string ConvertLinksToHttp(this string text)
        {
            return ConvertLinksToHttp(text, true);
        }

        public static string ConvertLinksToHttp(this string text, bool nofollow)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            if (nofollow)
            {
                text = Regex.Replace(text,
                                     @"(?<l>^|(\<br\s{0,1}/\>)|(\<br\>)|(\</{0,1}p\s{0,1}\>)|[^>^\x22^=^'^`^\[^\|^:])(?<link>http(?:s){0,1}://[^\.]+\.+[^\s^\n^\r^\]^\x22^'^`^\[^\|^:^\<^\>]*\.(jpg|bmp|png|gif))",
                                     "${l}<a href=\"${link}\" class=\"nflink\" target=\"_blank\" rel=\"nofollow\"><img border=\"0\" src=\"${link}\" class=\"imgMax\"/></a>",
                                     RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                text = Regex.Replace(text,
                                     @"(?<l>^|(\<br\s{0,1}/\>)|(\<br\>)|(\</{0,1}p\s{0,1}\>)|[^>^\x22^=^'^`^\[^\|^:])(?<link>http(?:s){0,1}://[^\.]+\.+[^\s^\n^\r^\]^\x22^'^`^\[^\|^:^\<^\>]*)",
                                     "${l}<a href=\"${link}\" class=\"nflink\" target=\"_blank\" rel=\"nofollow\">${link}</a>",
                                     RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            else
            {
                text = Regex.Replace(text,
                                     @"(?<l>^|(\<br\s{0,1}/\>)|(\<br\>)|(\</{0,1}p\s{0,1}\>)|[^>^\x22^=^'^`^\[^\|^:])(?<link>http(?:s){0,1}://[^\.]+\.+[^\s^\n^\r^\]^\x22^'^`^\[^\|^:^\<^\>]*\.(jpg|bmp|png|gif))",
                                     "${l}<a href=\"${link}\" target=\"_blank\"><img border=\"0\" src=\"${link}\" class=\"imgMax\"/></a>",
                                     RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                text = Regex.Replace(text,
                                     @"(?<l>^|(\<br\s{0,1}/\>)|(\<br\>)|(\</{0,1}p\s{0,1}\>)|[^>^\x22^=^'^`^\[^\|^:])(?<link>http(?:s){0,1}://[^\.]+\.+[^\s^\n^\r^\]^\x22^'^`^\[^\|^:^\<^\>]*)",
                                     "${l}<a href=\"${link}\" target=\"_blank\">${link}</a>",
                                     RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return text;
        }

        public static string StripHtmlXmlTags(this string content)
        {
            return StripHtmlXmlTagsRegex.Replace(content, "");
        }

        public static Regex StripHtmlXmlTagsRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}