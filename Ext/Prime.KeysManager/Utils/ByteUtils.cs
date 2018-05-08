using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.KeysManager.Utils
{
    public static class ByteUtils
    {
        public static string DecodeAscii(this byte[] buffer)
        {
            int count = Array.IndexOf<byte>(buffer, 0, 0);
            if (count < 0) count = buffer.Length;
            return Encoding.Default.GetString(buffer, 0, count);
        }
    }
}
