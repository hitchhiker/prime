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
            if (!path.StartsWith("[SRC]", StringComparison.OrdinalIgnoreCase))
                return path;

            var current = Path.GetFullPath("./");
            var io = current.IndexOf(Path.DirectorySeparatorChar + "src" + Path.DirectorySeparatorChar);
            if (io == 1)
                throw new Exception("Could not find 'src' folder in " + current);

            return current.Substring(0, io + 4) + path.Substring(5);
        }
    }
}