using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Prime.Utility
{
    public static partial class ExtensionMethods
    {
        //http://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
        public static string DeCamelCase(this string source)
        {
            return Regex.Replace(
                Regex.Replace(
                    source,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                    ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
                );
        }

        public static string DeEnum(this string source)
        {
            return source.Capitalise(DeCamelCase(source));
        }
               
        public static string ToDirectorySeparator(this string source, char seperator)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;
            if (seperator != '/' && source.IndexOf('/') != -1)
                source = source.Replace('/', seperator);
            if (seperator != '\\' && source.IndexOf('\\') != -1)
                source = source.Replace('\\', seperator);
            return source;
        }

        public static string ToVirtualDirectorySeparator(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;
            return source.IndexOf(Path.DirectorySeparatorChar) == -1 ? source : source.Replace(Path.DirectorySeparatorChar.ToString(), "/");
        }

        public static string ToFileSystemDirectorySeparator(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;
            if (Path.DirectorySeparatorChar == '/' || source.IndexOf('/') == -1)
                return source;
            return source.Replace("/", Path.DirectorySeparatorChar.ToString());
        }
        
        private static readonly ConcurrentDictionary<string, ObjectId> HashCache = new ConcurrentDictionary<string, ObjectId>();

        internal static Timer HashCacheClearTimer = new Timer(state => HashCache.Clear(), null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        
        /// <summary>
        /// Warning: May not return the same reference!
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static FileInfo RenameExt(this FileInfo source, string ext)
        {
            if (string.IsNullOrEmpty(ext))
                ext = "";
            else if (!ext.StartsWith("."))
                ext = "." + ext;

            var np = Path.ChangeExtension(source.FullName, ext);
            
            if (!source.Exists)
                return new FileInfo(np);

            var nfi = new FileInfo(np);
            if (nfi.FullName.Equals(source.FullName, StringComparison.OrdinalIgnoreCase))
                return source;

            if (nfi.Exists)
            {
                nfi.Delete();
            }

            source.MoveTo(nfi.FullName);
            source.Refresh();
            return source;
        }

        /// <summary>
        /// Warning: May not return the same reference!
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static FileInfo Rename(this FileInfo source, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var fp = source.FullName.Substring(0, source.FullName.Length - source.Name.Length);


            var np = Path.Combine(fp, name);

            if (!source.Exists)
                return new FileInfo(np);

            var nfi = new FileInfo(np);
            if (nfi.FullName.Equals(source.FullName, StringComparison.OrdinalIgnoreCase))
                return source;

            if (nfi.Exists)
            {
                nfi.Delete();
            }

            source.MoveTo(nfi.FullName);
            source.Refresh();
            return source;
        }

        public static string ToXmlString(this XDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            var builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                doc.Save(writer);
            }
            return builder.ToString();
        }
        
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (string.IsNullOrWhiteSpace(oldValue))
                return originalString;
            
            var startIndex = 0;
            var count = 0;
            while (true)
            {
                count++;
                if (count>10000)
                    throw new Exception("Possible infinite loop detected in 'Replace' with values '" + oldValue + "' '" + newValue +"'");

                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

        public static string CacheBreaker(this DateTime utcDateTime)
        {
            return utcDateTime.ToString("MdyyHmmss");
        }

        public static string ToUtcFormat(this DateTime utcDateTime)
        {
            return utcDateTime.ToString("MMM dd yyyy HH:mm:ss");
        }

        public static string ToXTime(this DateTime utcDateTime)
        {
            return "<span class=\"_ac _t\">" + utcDateTime.ToUtcFormat() + "</span>";
        }
        
        public static string ToJsonClass(this Dictionary<string, int> items) { return "|" + String.Join("_", items.Select(x => "-" + x.Key + "-:" + x.Value)); }

        /// <summary>
        /// Converts the list into a CSV string, ignores null values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToCsv<T>(this IEnumerable<T> list)
        {
            var t = list.Select(x=>x.ToString()).Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
            return t.Count == 0 ? "" : String.Join(",", t);
        }

        public static string ToCsvId<T>(this IEnumerable<T> list) where T : IUniqueIdentifier<ObjectId>
        {
            var t = list.Select(x => x.Id.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return t.Count == 0 ? "" : String.Join(",", t);
        }

        private static IEnumerable<string> ToCsvEnumerable(this string csv, char spliton = ',')
        {
            return String.IsNullOrEmpty(csv) ? new List<string>() : csv.Split(spliton).Where(x => !String.IsNullOrEmpty(x));
        }

        public static List<ObjectId> ToCsvId(this string csv, char spliton = ',')
        {
            return String.IsNullOrEmpty(csv) ? new List<ObjectId>() : csv.Split(spliton).Where(x => !String.IsNullOrEmpty(x)).Select(x=> x.ToObjectId()).ToList();
        }

        /// <summary>
        /// Converts the string into a List based on comma seperated values.
        /// </summary>
        public static List<string> ToCsv(this string csv, Func<string, string> postFilter = null, char spliton = ',')
        {
            return ToCsvEnumerable(csv, spliton).Select(x => postFilter != null ? postFilter(x) : x).ToList();
        }

        public static List<string> ToCsvFromNewline(this string csv, char safePattern = ',')
        {
            return ToCsv(csv.ChangeNewLine(safePattern.ToString()), true, safePattern);
        }

        public static List<string> ToCsv(this string csv, char spliton)
        {
            return ToCsv(csv, true, spliton);
        }

        public static List<string> ToCsv(this string csv, bool clean, char spliton = ',')
        {
            return !clean ? ToCsv(csv, null, spliton) : ToCsvEnumerable(csv, spliton).Select(x => x.Replace(Environment.NewLine, "").Replace('\r'.ToString(),"").Trim()).ToList();
        }

        public static List<int> ToCsvInt(this string csv, char spliton = ',')
        {
            return String.IsNullOrEmpty(csv) ? new List<int>() : csv.Split(spliton).Where(x => !String.IsNullOrEmpty(x) && x.IsNumeric()).Select(int.Parse).ToList();
        }

        public static List<decimal> ToCsvDecimal(this string csv, char spliton = ',')
        {
            return String.IsNullOrEmpty(csv) ? new List<decimal>() : csv.Split(spliton).Where(x => !String.IsNullOrEmpty(x) && x.IsNumeric()).Select(decimal.Parse).ToList();
        }

        public static List<double> ToCsvDouble(this string csv, char spliton = ',')
        {
            return String.IsNullOrEmpty(csv) ? new List<double>() : csv.Split(spliton).Where(x => !String.IsNullOrEmpty(x) && x.IsNumeric()).Select(double.Parse).ToList();
        }

        /// <summary>
        /// Safely gets a given attribute value, or returns null
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Attribute(this XmlNode node, string name)
        {
            if (node.Attributes == null || node.Attributes.Count == 0)
                return null;

            var attr = node.Attributes[name];
            if (attr!=null && !String.IsNullOrEmpty(attr.Value))
                return attr.Value.Trim();
            return null;
        }

        public static int WordCount(this String str)
        {
            return str.Split(new[] {' ', '.', '?'}, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string ToCompactString(this JObject jobject)
        {
            return JsonConvert.SerializeObject(jobject, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// Counts the occurances of a string within a string
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="checkString"></param>
        /// <returns></returns>
        public static int Count(this string inputString, string checkString)
        {
            if (String.IsNullOrEmpty(inputString) || String.IsNullOrEmpty(checkString))
                return 0;

            if (checkString.Length > inputString.Length )
                return 0;
            
            var lengthDifference = inputString.Length - checkString.Length;
            var occurencies = 0;
            for (var i = 0; i <= lengthDifference; i++)
            {
                if (!inputString.Substring(i, checkString.Length).Equals(checkString))
                    continue;
                occurencies++;
                i += checkString.Length - 1;
            }
            return occurencies;
        }

        public static bool ContainsIgnoreCase(this string inputString, string checkString)
        {
            return !String.IsNullOrEmpty(inputString) && inputString.Contains(checkString, StringComparison.OrdinalIgnoreCase);
        }

        public static string Trim(this string target, string trimString)
        {
            return String.IsNullOrEmpty(target) ? target : target.TrimStart(trimString).TrimEnd(trimString);
        }

        public static string TrimStart(this string target, string trimString)
        {
            if (String.IsNullOrEmpty(target))
                return target;

            var result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }
            return result;
        }

        public static string TrimEnd(this string target, string trimString)
        {
            if (String.IsNullOrEmpty(target))
                return target;

            var result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static DateTime ChangedUtc(this FileInfo fileInfo)
        {
            return fileInfo.LastWriteTimeUtc > fileInfo.CreationTimeUtc ? fileInfo.LastWriteTimeUtc : fileInfo.CreationTimeUtc;
        }

        /// <summary>
        /// Compares the strings in the supplied set to ensure they all exist in the source set.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comparedTo"></param>
        /// <param name="stringComparisonType"></param>
        /// <returns></returns>
        public static bool All(this IEnumerable<string> source, IEnumerable<string> comparedTo, StringComparison stringComparisonType = StringComparison.Ordinal)
        {
            return source.All(x => comparedTo.Any(s => s.Equals(x, stringComparisonType)));
        }

        /// <summary>
        /// Compares the strings in the supplied sets and ensures they are identical
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comparedTo"></param>
        /// <param name="stringComparisonType"></param>
        /// <returns></returns>
        public static bool Match(this IEnumerable<string> source, IEnumerable<string> comparedTo, StringComparison stringComparisonType = StringComparison.Ordinal)
        {
            var src = source.ToArray();
            var comp = comparedTo.ToArray();
            return src.Length == comp.Length && src.All(comp, stringComparisonType);
        }
        
        public static string ChangeNewLine(this string value, string newString)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return value.Replace("\r", "\n").Replace("\n\n", "\n").Replace("\n", newString);
        }
    }
}
