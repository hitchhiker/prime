using System;
using System.Text;

namespace Prime.Core
{
    public static class BytesEncoderExtensions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static string DecodeString(this byte[] buffer)
        {
            var count = Array.IndexOf<byte>(buffer, 0, 0);
            if (count < 0) count = buffer.Length;
            return Encoding.Default.GetString(buffer, 0, count);
        }
    }
}