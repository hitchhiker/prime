using System;
using System.IO;
using System.Reflection;
using SevenZip;

namespace Prime.Radiant
{
    public static class SevenZipNativeLibrary
    {
        public static LibraryFeature Install()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "7z-x64.dll" : "7z-x86.dll");
            SevenZipBase.SetLibraryPath(path);
            return new LibraryFeature();
        }
    }
}