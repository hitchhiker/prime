#region

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

#endregion

//http://www.codeproject.com/KB/files/GZipStream.aspx
//http://social.msdn.microsoft.com/Forums/en/netfxbcl/thread/093b4906-3b26-493d-8d90-fc527d433c7c

namespace Prime.Utility
{
    public static class CompressedUtf8String
    {
        public static byte[] Compress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return new byte[0];

            if (s.Length < 150) //approximation guess of the efficiency of gzip on text/html
               return ReturnWithoutCompression(s);

            var stringlength = (s.Length * 2) + 1; //utf8 + header

            using (var mStore = new MemoryStream())
            {
                mStore.WriteByte(1); //our header, saying it's gzip
                using (var strm = new GZipStream(mStore, CompressionMode.Compress, true))
                {
                    var enc = Encoding.Unicode.GetBytes(s);
                    strm.Write(enc, 0, enc.Length);
                }
                return mStore.Length >= stringlength ? ReturnWithoutCompression(s) : mStore.ToArray();
            }
        }

        private static byte[] ReturnWithoutCompression(string s)
        {
            var h = new byte[] {0};
            var b = Encoding.Unicode.GetBytes(s);
            return h.Concat(b).ToArray();
        }

        public static string Expand(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length <2)
                return "";

            if (byteArray[0] == 0) //it's not compressed
                return new String(Encoding.Unicode.GetChars(byteArray, 1, byteArray.Length - 1));

            var mStore = new MemoryStream(byteArray);
            mStore.Seek(1, SeekOrigin.Begin);
            using (var strm = new GZipStream(mStore, CompressionMode.Decompress, true))
            {
                return Encoding.Unicode.GetString(ReadFully(strm));
            }
        }

        private static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}