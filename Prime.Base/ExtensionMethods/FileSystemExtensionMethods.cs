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
    }
}