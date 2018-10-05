using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Prime.Bootstrap
{
    public class Bootstrapper
    {
        public static string CoreBaseDirPath = @"{0}\package\install\prime-f7015e4f838b8f7439722bb6\";
        public static string CoreBasePath = CoreBaseDirPath + @"{1}\Prime.Core.dll";

        public static string TagA = "<installed entry=\"";
        public static string TagB = "\">";

        public static void Boot(string[] args, string execName)
        {
            if (BootstrapperUpgrade.Check(args, execName))
            {
                Console.WriteLine("BOOTSTRAP: Bootstrap exiting during upgrade.");
                return;
            }

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

            Run(confp, argl.ToArray());
        }

        private static void Error()
        {
            Console.WriteLine("You must specify the location of the .config file.");
            Console.WriteLine("");
            Console.WriteLine("example: -c ../instance/prime.config");
        }

        private static int Run(string configPath, string[] args)
        {
            var configp = Utilities.ResolveSpecial(configPath);

            var configFi = new FileInfo(configp);

            if (!configFi.Exists)
            {
                Console.WriteLine("BOOTSTRAP: " + configp + " does not exist.");
                return 1;
            }

            var confDir = configFi.Directory.FullName;
            var confName = configFi.Name.Replace(configFi.Extension, "");

            var entryVersion = GetEntryVersion(File.ReadAllText(configp));
            if (string.IsNullOrWhiteSpace(entryVersion))
            {
                Console.WriteLine("BOOTSTRAP: Entry point missing from .config file. Finding latest..");

                var rdir = Utilities.FixDir(Path.Combine(confDir, string.Format(CoreBaseDirPath, confName)));
                var dir = new DirectoryInfo(rdir);

                if (!dir.Exists) { 
                    Console.WriteLine("BOOTSTRAP: No packages found at: " + dir.FullName);
                    return 1;
                }

                var dirs = dir.EnumerateDirectories().ToList();
                if (dirs.Count==0)
                {
                    Console.WriteLine("BOOTSTRAP: No packages found, directory empty at: " + dir.FullName);
                    return 1;
                }

                var latest = dir.EnumerateDirectories().OrderByDescending(x => x.Name).FirstOrDefault();
                entryVersion = latest.Name;
                Console.WriteLine("BOOTSTRAP: Scanning location: " + dir.FullName);
                Console.WriteLine("BOOTSTRAP: Trying version: " + entryVersion);
            }

            var primeCorePath = Path.Combine(confDir, string.Format(CoreBasePath, confName, entryVersion));
            primeCorePath = Utilities.FixDir(primeCorePath);

            var primeCore= new FileInfo(primeCorePath);

            if (!primeCore.Exists)
            {
                Console.WriteLine("BOOTSTRAP: " + primeCore.FullName + " not found.");
                return 1;
            }

            var asm = Utilities.LoadAssemblyLegacy(primeCore.FullName);

            var t = asm.GetType("Prime.Core.BootstrapperEntry");
            if (t == null)
            {
                Console.WriteLine("BOOTSTRAP: Unable to find Prime's entry class in: " + primeCore.FullName);
                return 1;
            }

            var mi = t.GetMethod("Enter", BindingFlags.Static | BindingFlags.Public);
            if (mi == null)
            {
                Console.WriteLine("BOOTSTRAP: Unable to find Prime's entry method in: " + primeCore.FullName);
                return 1;
            }

            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("BOOTSTRAP: " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.ForegroundColor = c;

            mi.Invoke(null, new object[] {args, configPath});
            return 0;
        }

        public static string GetEntryVersion(string configText)
        {
            var i1 = configText.IndexOf(TagA, 0, StringComparison.OrdinalIgnoreCase);
            if (i1 == -1)
                return null;
            i1 = i1 + TagA.Length;
            var i2 = configText.IndexOf(TagB, i1, StringComparison.OrdinalIgnoreCase);
            return i1 == -1 ? null : configText.Substring(i1, i2 - i1);
        }
    }
}
