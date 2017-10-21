using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class ManagedFileSystem
    {
        private ManagedFileSystem() {}

        public static ManagedFileSystem I => Lazy.Value;
        private static readonly Lazy<ManagedFileSystem> Lazy = new Lazy<ManagedFileSystem>(()=>new ManagedFileSystem());

        public FileInfo Get(IDataContext context, string key)
        {
            var hash = key.GetObjectIdHashCode(true, true);
            var path = Path.Combine(Path.Combine(context.StorageDirectory.FullName, "fs"), GetPath(hash));
            return new FileInfo(path);
        }

        private static string GetPath(ObjectId id)
        {
            var ids = id.ToString();
            return ids[0] + Path.DirectorySeparatorChar.ToString() + ids[1] + Path.DirectorySeparatorChar + id;
        }
        /*
        public BitmapSource ImageFrom(IDataContext context, Uri uri)
        {
            var fi = Get(context, "uri:" + uri);

            if (fi.Exists)
            {
                using (var s = fi.OpenRead())
                {
                    return FromStream(s);
                }
            }

            using (var ms = Download(uri))
            {
                if (!fi.Directory.Exists)
                    fi.Directory.Create();

                File.WriteAllBytes(fi.FullName, ms.ToArray());
                return FromStream(ms);
            }
        }
        */

        private static MemoryStream Download(Uri uri)
        {
            var request = WebRequest.CreateDefault(uri);
            var buffer = new byte[4096];
            var target = new MemoryStream();

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        target.Write(buffer, 0, read);
                    }
                }
            }
            return target;
        }
        /*
        private static BitmapSource FromStream(Stream byteStream)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = byteStream;
            bi.EndInit();

            return bi;
        }*/
    }
}