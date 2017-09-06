using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using LiteDB;

namespace Prime.Utility
{
    public static class EnumExtensionMethods
    {
        /// <summary>
        /// Extracts a rnd value from an enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RndEnum<T>() where T : struct, IConvertible
        {
            var t = typeof (T);
            if (!t.GetTypeInfo().IsEnum)
                throw new ArgumentException("The RndEnum<T> extension method is used for Enums only.");

            return Enum.GetValues(typeof (T)).Cast<T>().OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        public static ObjectId GetObjectIdHashCode(this Enum e)
        {
            return Obcache.GetOrAdd(e, @enum => @enum.ToString().ToLower().GetObjectIdHashCode());
        }

        private readonly static ConcurrentDictionary<Enum, ObjectId> Obcache = new ConcurrentDictionary<Enum, ObjectId>(); 
    }
}