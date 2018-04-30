using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using LiteDB;

namespace Prime.Core
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
        
    }
}