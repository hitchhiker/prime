using System;
using System.IO;
using System.Linq;

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

        public static bool FileOrDirExists(this string path)
        {
            return (Directory.Exists(path) || File.Exists(path));
        }

        /// <summary>
        /// Determines the type of object in the filesystem at the given path.
        /// 0=Does not exist, 1=File, 2=Directory
        /// </summary>
        public static int DetectFileSystemPath(this string path)
        {
            if (File.Exists(path))
                return 1;

            if (Directory.Exists(path))
                return 2;

            return 0;
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
            var s = FindParent(path, "src");
            s = DoSpecial(s, "USER", ()=> Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
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
            if (io != -1)
                return current.Substring(0, io + special.Length + 1) + path.Substring(special.Length + 2);

            var p = FindParent(new DirectoryInfo("./"), special);
            if (p != null)
                return p + path.Substring(special.Length + 2);

            throw new Exception("Could not find '[" + special + "]' special folder in " + current);
        }

        private static string FindParent(DirectoryInfo child, string special)
        {
            if (child == null || !child.Exists)
                return null;
            return child.GetFiles("." + special).Any() ? child.FullName : FindParent(child.Parent, special);
        }
    }
}