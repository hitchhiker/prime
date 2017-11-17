using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Prime.Utility
{
    public static class DictionaryExtensions
    {
        public static T2 Get<T1, T2>(this IReadOnlyCollection<KeyValuePair<T1, T2>> col, T1 key)
        {
            if (col is IReadOnlyDictionary<T1, T2> rd)
                return rd.Get(key);

            if (col is Dictionary<T1, T2> d)
                return d.Get(key);

            foreach (var kv in col)
                if (Equals(kv.Key, key))
                    return kv.Value;

            return default;
        }

        private static T2 Get<T1, T2>(this IReadOnlyDictionary<T1, T2> dict, T1 key)
        {
            if (dict == null || dict.Count == 0)
                return default(T2);
            return dict.ContainsKey(key) ? dict[key] : default(T2);
        }

        private static T2 Get<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
        {
            if (dict == null || dict.Count == 0)
                return default(T2);
            return dict.ContainsKey(key) ? dict[key] : default(T2);
        }

        public static object Get(this IDictionary<string, object> dict, string key, object defaultValue = null)
        {
            if (dict == null || dict.Count == 0)
                return defaultValue;
            return dict.ContainsKey(key) ? ((dict[key]) ?? defaultValue) : defaultValue;
        }

        public static T1 Get<T1>(this IDictionary<string, object> dict, string key, T1 defaultValue = default(T1))
        {
            if (dict == null || dict.Count == 0)
                return defaultValue;
            
            if (!dict.ContainsKey(key))
                return defaultValue;

            var o = dict[key];
            if (!(o is T1))
                return defaultValue;
            return (T1) o;
        }

        public static T GetAs<T>(this IDictionary<string, T> dict, string key, T defaultValue = null) where T : class
        {
            if (dict == null || dict.Count == 0)
                return defaultValue;
            return dict.ContainsKey(key) ? dict[key] as T : defaultValue;
        }

        public static T Get<T>(this IDictionary<string, T> dict, string key, T defaultValue = default(T))
        {
            if (dict == null || dict.Count == 0)
                return defaultValue;
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }
        
        public static T2 Get<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 defaultValue)
        {
            if (dict == null || dict.Count == 0)
                return defaultValue;
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }

        public static T2 GetOrAdd<T1, T2>(this IDictionary<T1, T2> dict, T1 key, Func<T1, T2> valueCreate)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            var val = valueCreate(key);
            dict.Add(key, val);
            return val;
        }

        /// <summary>
        /// If they key exists, it is retrieved, otherwise it is added.
        /// </summary>
        public static T2 GetOrCreate<T1, T2>(this IDictionary<T1, T2> dict, T1 key, Func<T2> valueCreate)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            var val = valueCreate();
            dict.Add(key, val);
            return val;
        }

        public static bool Add<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 value, bool ignoreDefault = false)
        {
            if (ignoreDefault && Equals(value, default(T2)))
                return false;

            dict.Add(key, value);
            return true;
        }

        /// <summary>
        /// If they key exists, it is updated, otherwise it is added.
        /// </summary>
        public static bool CreateOrUpdate<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 value, bool ignoreDefault = false)
        {
            if (ignoreDefault && Equals(value, default(T2)))
                return false;

            if (dict.ContainsKey(key))
            {
                dict[key] = value;
                return false;
            }

            dict.Add(key, value);
            return true;
        }

        /// <summary>
        /// If they key exists, it is updated, otherwise it is added.
        /// </summary>
        public static T2 CreateOrUpdate<T1, T2>(this IDictionary<T1, T2> dict, T1 key, Func<T2> valueCreate)
        {

            if (dict.ContainsKey(key))
                dict.Remove(key);

            var val = valueCreate();
            dict.Add(key, val);
            return val;
        }

        /// <summary>
        /// If the key already exists, nothing is done, and false is returned.
        /// </summary>
        public static bool AddOnce<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 value, bool ignoreDefault = false)
        {
            if (ignoreDefault && Equals(value, default(T2)))
                return false;

            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }

        /// <summary>
        /// Same as ADD: Upserts the key, if the value is the 'default value' - the key will be removed.
        /// Warning, if using int be aware that the default value of an int is 0, so specify a different default if working with values of 0.
        /// If the key exists, it will be updated
        /// </summary>
        public static void AddOrUpdateNoDefault<T1,T2>(this IDictionary<T1, T2> dict, T1 key, T2 value, T2 defaultValue = default(T2))
        {
            var def = Equals(value, default(T2));

            if (!def)
                dict.CreateOrUpdate(key, value);
            else if (dict.ContainsKey(key))
                dict.Remove(key);
        }

        public static bool AddOnceWith<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 value, Func<T2, T2> func)
        {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, func(value));
            return true;
        }

        public static bool AddOrUpdateWith<T1, T2>(this IDictionary<T1, T2> dict, T1 key, Func<T2> func)
        {
            if (dict.ContainsKey(key))
                dict.Remove(key);

            dict.Add(key, func());
            return true;
        }

        public static bool AddOrUpdate<T1, T2>(this IDictionary<T1, T2> dict, T1 key, Func<T1, T2> func)
        {
            if (dict.ContainsKey(key))
                dict.Remove(key);

            dict.Add(key, func(key));
            return true;
        }

        public static ObjectId GetId(this IDictionary<string, string> dict, string name, ObjectId defaultValue = null)
        {
            return dict.Get(name, "").ToObjectId(defaultValue);
        }

        public static Type GetType(this IDictionary<string, string> dict, string name)
        {
            var i = dict.GetIntN(name);
            return i == null ? null : TypeCatalogue.I.Get(i);
        }

        public static T GetInstance<T>(this IDictionary<string, string> dict, string name) where T : class
        {
            var i = dict.GetIntN(name);
            if (i == null)
                return null;
            var t = TypeCatalogue.I.Get(i);
            return t == null ? null : t.InstanceAny<T>();
        }

        public static List<T> GetMany<T>(this IDictionary<string, string> dict, string name, char seperator, Func<string, T> creator) where T : class
        {
            var i = dict.Get(name, null);
            return string.IsNullOrEmpty(i) ? new List<T>() : i.Split(seperator).Select(creator).Where(x=>x!=null).ToList();
        }

        public static bool GetBool(this IDictionary<string, string> dict, string name, bool defaultValue = false)
        {
            return dict.Get(name, "").ToBool(defaultValue);
        }

        public static bool? GetBoolN(this IDictionary<string, string> dict, string name, bool? defaultValue = null)
        {
            return dict.Get(name, "").ToBoolN(defaultValue);
        }

        public static Guid GetGuid(this IDictionary<string, string> dict, string name)
        {
            return dict.Get(name, "").ToGuid();
        }

        public static Guid? GetGuid(this IDictionary<string, string> dict, string name, Guid? defaultValue)
        {
            return dict.Get(name, "").ToGuid(defaultValue);
        }

        public static int GetInt(this IDictionary<string, string> dict, string name, int defaultValue = 0)
        {
            return dict.Get(name, "").ToInt(defaultValue);
        }

        public static int? GetIntN(this IDictionary<string, string> dict, string name, int? defaultValue = null)
        {
            return dict.Get(name, "").ToIntN(defaultValue);
        }

        public static decimal GetDecimal(this IDictionary<string, string> dict, string name, decimal defaultValue = 0)
        {
            return dict.Get(name, "").ToDecimal(defaultValue);
        }

        public static decimal? GetDecimalN(this IDictionary<string, string> dict, string name, decimal? defaultValue = null)
        {
            return dict.Get(name, "").ToDecimalN(defaultValue);
        }

        public static double GetDouble(this IDictionary<string, string> dict, string name, double defaultValue = 0)
        {
            return dict.Get(name, "").ToDouble(defaultValue);
        }

        public static double? GetDoubleN(this IDictionary<string, string> dict, string name, double? defaultValue = null)
        {
            return dict.Get(name, "").ToDoubleN(defaultValue);
        }
        public static long GetLong(this IDictionary<string, string> dict, string name, long defaultValue = 0)
        {
            return dict.Get(name, "").ToLong(defaultValue);
        }

        public static long? GetLongN(this IDictionary<string, string> dict, string name, long? defaultValue = null)
        {
            return dict.Get(name, "").ToLongN(defaultValue);
        }

        public static DateTime GetDate(this IDictionary<string, string> dict, string name, DateTime? defaultValue = null)
        {
            return dict.Get(name, "").ToDate(defaultValue);
        }

        public static DateTime GetDateTime(this IDictionary<string, string> dict, string name, DateTime? defaultValue = null)
        {
            return dict.Get(name, "").ToDateTime(defaultValue);
        }
        
        public static T GetEnum<T>(this IDictionary<string, string> dict, string name, T defaultValue = default(T)) where T : struct
        {
            return dict.Get(name, "").ToEnum(defaultValue);
        }

        public static void RemoveAll<TK, T>(this IDictionary<TK, T> dictionary, Func<KeyValuePair<TK, T>, bool> predicate)
        {
            var removekeys = dictionary.Where(predicate).Select(x => x.Key).ToList();
            foreach (var k in removekeys)
                dictionary.Remove(k);
        }

        public static bool ContainsKey<T>(this IDictionary<ObjectId, T> dictionary, IUniqueIdentifier<ObjectId> doc)
        {
            return dictionary.ContainsKey(doc.Id);
        }

        public static void CopyKey<T>(this Dictionary<string, T> source, string key, string destinationKey)
        {
            if (source?.ContainsKey(key) != true)
                return;

            source.AddOrUpdate(destinationKey, (k) => source.Get(key));
        }

        public static void CopyKey<T>(this IDictionary<string, T> source, string key, string destinationKey)
        {
            CopyKey(source, source, key, destinationKey);
        }

        public static void CopyKey<T>(this IDictionary<string, T> source, IDictionary<string, T> destination, string key, string destinationKey)
        {
            if (source?.ContainsKey(key) != true)
                return;

            destination?.AddOrUpdate(destinationKey, (k) => source.Get(key));
        }

        public static KeyValuePair<T1,T2> GetRandom<T1, T2>(this IDictionary<T1, T2> dict)
        {
            if (dict == null || dict.Count == 0)
                return new KeyValuePair<T1,T2>();

            var rnd = Rnd.I.Next(dict.Count);
            return rnd == 0 ? dict.FirstOrDefault() : dict.Skip(rnd).FirstOrDefault();
        }
    }
}
