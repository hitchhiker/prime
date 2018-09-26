using System;
using System.Runtime.InteropServices;

namespace Prime.Core
{
    public static class OsInformation
    {
        public static Platform GetPlatform()
        {
            var is64 = Environment.Is64BitOperatingSystem;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return is64 ? Platform.WinAmd64 : Platform.Win386;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return is64 ? Platform.LinuxAmd64 : Platform.Linux386;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return is64 ? Platform.Darwin386 : Platform.DarwinAmd64;

            return Platform.NotSpecified;
        }
    }
}