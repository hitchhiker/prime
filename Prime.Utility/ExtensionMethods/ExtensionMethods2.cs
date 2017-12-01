using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LiteDB;
using System.Diagnostics;

//mod from: http://aonnull.blogspot.com/2010/08/dynamic-sql-like-linq-orderby-extension.html

namespace Prime.Utility
{
    public static partial class ExtensionMethods
    {
        public static bool Contains(this string source, string item, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            return source.IndexOf(item, comparisonType) > -1;
        }

        public static bool Contains(this IEnumerable<string> items, string item, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            return string.IsNullOrEmpty(item) ? items.Any(string.IsNullOrEmpty) : items.Any(x => item.Equals(x, comparisonType));
        }
        
        public static ObjectId EmptyObjectId => new ObjectId();

        /// <summary>
        /// Get's the default value of either the reference type (null) or the given value type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
        
        public static bool IsEmpty(this ObjectId objectid)
        {
            return Equals(objectid, ObjectId.Empty);
        }

        public static bool IsNullOrEmpty(this ObjectId objectid)
        {
            return objectid== null || Equals(objectid, ObjectId.Empty);
        }

        public static string ToWebString(this ObjectId objectid)
        {
            return objectid.ToString();
        }
        
        /// <summary>
        /// Returns the first item of type T or the default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T FirstOf<T>(this IEnumerable items) where T : class
        {
            return items?.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns a random item or default(T) from the set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T RandomOrDefault<T>(this IEnumerable<T> items)
        {
            return items == null ? default(T) : items.OrderBy(x=> Guid.NewGuid()).FirstOrDefault();
        }

        public static IEnumerable<T> OrderByRandom<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(x => Guid.NewGuid());
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> items)
        {
            var i = items.ToList();
            var t = i.Count();
            return t == 0 ? i : i.Take(Rnd.I.Next(1, t));
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> items, int minimum, int percentageWin = 50)
        {
            var its = items.ToList();
            var t = its.Count();
            return t == 0 ? its : its.OrderByRandom().Where(i => minimum-- > 0 || Rnd.I.DiceRoll(percentageWin)).ToList();
        }

        public static IEnumerable<T> NoNulls<T>(this IEnumerable<T> items) where T : class
        {
            return items.Where(x => x != null);
        }

        public static MemoryStream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// Shortcut to format with 'n0', resulting in a comma delimited (eg: 100,000,000) representation.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToHtml(this int number)
        {
            return number.ToString("n0");
        }

        /// <summary>
        /// http://stackoverflow.com/a/10380166/1318333
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// http://stackoverflow.com/a/10380166/1318333
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToString(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static void Add(this ICollection<ObjectId> list, IUniqueIdentifier<ObjectId> document, bool ignoreNull = true)
        {
            if (document == null)
                if (ignoreNull)
                    return;
                else
                    throw new ArgumentException("Cannot add the ID of a null object to the list.");
            list.Add(document.Id);
        }

        public static void DoAs<T>(this object ob, Action<T> func) where T : class
        {
            var type = ob as T;
            if (type == null)
                return;

            func(type);
        }

        public static string ToElapsed(this Stopwatch stopwatch, bool seconds = true)
        {
            var ts = stopwatch.Elapsed;
            if (seconds)
                return $"{ts.Seconds:0}.{ts.Milliseconds / 10:00}s";
            return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        }
    }
}
