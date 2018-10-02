using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prime.Bootstrap
{
    public class Bootstrapper
    {
        public static void Boot(string[] args)
        {
            var argl = args.ToList();
            var io = argl.IndexOf("-c");
            if (io == -1 || io == argl.Count - 1)
            {
                Error();
                return;
            }

            var confp = argl[io + 1];
            if (string.IsNullOrWhiteSpace(confp))
            {
                Error();
                return;
            }

            argl.RemoveAt(io + 1);
            argl.RemoveAt(io);

            Run(confp, args);
        }

        private static void Error()
        {
            Console.WriteLine("You must specify the location of the .config file.");
            Console.WriteLine("");
            Console.WriteLine("example: -c ../instance/prime.config");
        }

        private static int Run(string configPath, string[] args)
        {
            var configp = ResolveSpecial(configPath);

            if (!File.Exists(configp))
            {
                Console.WriteLine(configp + " does not exist.");
                return 1;
            }

            var entry = GetEntry(File.ReadAllText(configp));
            if (string.IsNullOrWhiteSpace(entry))
            {
                Console.WriteLine("Entry point missing from .config file.");
                return 1;
            }

            var primeCore = Path.Combine(entry, "Prime.Core.dll");
            if (!File.Exists(primeCore))
            {
                Console.WriteLine(primeCore + " not found.");
                return 1;
            }

            var asm = LoadAssemblyLegacy(primeCore);

            var t = asm.GetType("Prime.Core.BootstrapperEntry");
            if (t == null)
            {
                Console.WriteLine("Unable to find Prime's entry class in: " + primeCore);
                return 1;
            }

            var mi = t.GetMethod("Enter", BindingFlags.Static | BindingFlags.Public);
            if (mi == null)
            {
                Console.WriteLine("Unable to find Prime's entry method in: " + primeCore);
                return 1;
            }
            
            Console.WriteLine("Args: " + string.Join(" ", args));

            mi.Invoke(null, new object[] {args, configPath});
            return 0;
        }

        public static string GetEntry(string configText)
        {
            var i1 = configText.IndexOf("<entry>", 0, StringComparison.OrdinalIgnoreCase);
            if (i1 == -1)
                return null;
            var i2 = configText.IndexOf("</entry>", i1, StringComparison.OrdinalIgnoreCase);
            return i1 == -1 ? null : configText.Substring(i1 + 7, i2 - i1 - 7);
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
    }
}
