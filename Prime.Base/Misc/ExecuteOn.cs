using System;

namespace Prime.Core
{
    public static class ExecuteOn
    {
        public static T Windows<T>(Func<T> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            return Os(method, null);
        }

        public static T Unix<T>(Func<T> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            return Os(null, method);
        }

        public static T Os<T>(Func<T> windowsMethod, Func<T> unixMethod)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    return unixMethod != null ? unixMethod(): default(T);
                    break;
                case PlatformID.Win32NT:
                    return windowsMethod != null ? windowsMethod(): default(T);
                    break;
                case PlatformID.MacOSX:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}