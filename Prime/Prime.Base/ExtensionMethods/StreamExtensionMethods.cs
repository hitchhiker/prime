using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public static class StreamExtensionMethods
    {
        public static string ToX2String(this IEnumerable<byte> b)
        {
            var o = String.Empty;
            foreach (byte b1 in b)
            {
                o += b1.ToString("X2");
            }
            return o;
        }

        public static string DecodeUtf32(this byte[] buffer)
        {
            var count = Array.IndexOf<byte>(buffer, 0, 0);
            if (count < 0) count = buffer.Length;
            return Encoding.UTF32.GetString(buffer, 0, count);
        }
    }
}
