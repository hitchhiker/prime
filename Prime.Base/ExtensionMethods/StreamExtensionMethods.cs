using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public static class StreamExtensionMethods
    {
        public static string ToX2String(this IEnumerable<byte> b)
        {
            var o = string.Empty;
            foreach (byte b1 in b)
            {
                o += b1.ToString("X2");
            }
            return o;
        }
    }
}
