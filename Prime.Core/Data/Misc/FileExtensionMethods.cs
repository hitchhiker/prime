using System;
using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;

namespace Prime.Core
{
    public static class FileExtensionMethods
    {
        public static BitmapSource ToImageFromUri(this string uri, IDataContext context)
        {
            return string.IsNullOrWhiteSpace(uri) ? null : ToImageFrom(new Uri(uri), context);
        }

        public static BitmapSource ToImageFrom(this Uri uri, IDataContext context)
        {
            return ManagedFileSystem.I.ImageFrom(context, uri);
        }

    }
}