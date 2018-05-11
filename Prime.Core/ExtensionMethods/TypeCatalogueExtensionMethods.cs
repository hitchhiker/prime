using System;
using System.Collections.Generic;

namespace Prime.Core
{
    public static class TypeCatalogueExtensionMethods
    {
        public static Type GetType(this IDictionary<string, string> dict, string name)
        {
            var i = dict.GetIntN(name);
            return i == null ? null : ServerContext.Testing.Extensions.Types.Get(i);
        }

        public static T GetInstance<T>(this IDictionary<string, string> dict, string name) where T : class
        {
            var i = dict.GetIntN(name);
            if (i == null)
                return null;
            var t = ServerContext.Testing.Extensions.Types.Get(i);
            return t == null ? null : t.InstanceAny<T>();
        }
    }
}