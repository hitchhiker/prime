using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prime.Bootstrap
{
    public class Utilities
    {
        public static string FixDir(string dir)
        {
            dir = dir.Replace(@"/", Path.DirectorySeparatorChar.ToString());
            dir = dir.Replace(@"\", Path.DirectorySeparatorChar.ToString());
            return dir;
        }

        public static string ResolveSpecial(string path)
        {
            var s = FindParent(path, "src");
            s = DoSpecial(s, "USER", () => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
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

        public static Assembly LoadAssemblyLegacy(string dll)
        {
            try
            {
                var a = Assembly.LoadFrom(dll);
                return a;
            }
            catch (FileLoadException loadEx)
            {
                var x = loadEx;
            } // The Assembly has already been loaded.
            catch
            {
            } // If a BadImageFormatException exception is thrown, the file is not an assembly.
            return null;
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            
            foreach (var fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }
}