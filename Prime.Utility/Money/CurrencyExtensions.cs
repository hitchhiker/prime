using System;
using System.Collections.Generic;

namespace xC.Core
{
    internal static class CurrencyExtensions
    {
        public static String GetValueOrDefault(this IDictionary<String, String> table, String key)
        {
            String value;

            return !table.TryGetValue(key, out value) ? null : value;
        }
    }
}