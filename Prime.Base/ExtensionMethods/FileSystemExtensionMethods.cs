using System;
using System.IO;

namespace Prime.Core
{
    public static class FileSystemExtensionMethods
    {
        public static DirectoryInfo EnsureSubDirectory(this DirectoryInfo dir, string directoryName)
        {
            var di = new DirectoryInfo(Path.Combine(dir.FullName, directoryName));
            if (!di.Exists)
                di.Create();

            return di;
        }

        public static DirectoryInfo EnsureTempSubDirectory(this DirectoryInfo dir)
        {
            return dir.EnsureSubDirectory(RandomText.RandomFastString(10));
        }

        public static string GetFullPath(this string path, DirectoryInfo root)
        {
            return !path.StartsWith(".") ? path : Path.Combine(root.FullName, path);
        }

        public static string ResolveSpecial(this string path)
        {
            var s = FindParent(path, "SRC");
            s = DoSpecial(s, "USER", ()=> System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            return s;
        }

        private static string DoSpecial(string path, string special, Func<string> replacement)
        {
            if (!path.Contains("["))
                return path;

            var token = "[" + special + "]";

            return path.Replace(token, replacement());
        }

        private static string FindParent(string path, string special)
        {
            if (!path.StartsWith("[" + special + "]", StringComparison.OrdinalIgnoreCase))
                return path;

            var current = Path.GetFullPath("./");
            var io = current.IndexOf(Path.DirectorySeparatorChar + special + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            if (io == 1)
                throw new Exception("Could not find '[" + special + "]' special folder in " + current);

            return current.Substring(0, io + special.Length + 1) + path.Substring(special.Length + 2);
        }
    }
}