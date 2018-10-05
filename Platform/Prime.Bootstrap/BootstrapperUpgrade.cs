using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Prime.Bootstrap
{
    public class BootstrapperUpgrade
    {
        public static bool Check(string[] args, string execName)
        {
            var currentAsm = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var currentDirName = currentAsm.Directory.Name;

            if (currentDirName.EndsWith("_upgrade"))
                return PerformUpgrade(args, execName, currentAsm.Directory);

            var parentDir = currentAsm.Directory.Parent;
            if (parentDir == null)
                return false;

            return StartUpgrade(args, execName, parentDir, currentDirName);
        }

        private static bool StartUpgrade(string[] args, string execName, DirectoryInfo parentDir, string currentDirName)
        {
            var upgradeDir = new DirectoryInfo(Path.Combine(parentDir.FullName, currentDirName + "_upgrade"));
            if (!upgradeDir.Exists)
                return false;

            if (File.Exists(Path.Combine(upgradeDir.FullName, ".prime-upgraded")))
            {
                upgradeDir.Delete(true);
                Console.WriteLine("BOOTSTRAP: Cleaned up after upgrade of Bootstrapper.");
                return false;
            }

            Console.WriteLine("BOOTSTRAP: Starting upgrade of Bootstrapper..");
            Console.WriteLine("BOOTSTRAP: Starting: " + Path.Combine(upgradeDir.FullName, execName) + " " + string.Join(" ", args));
            Process.Start(Path.Combine(upgradeDir.FullName, execName), string.Join(" ", args));

            return true;
        }

        private static bool PerformUpgrade(string[] args, string execName, DirectoryInfo currentDir)
        {
            Console.WriteLine("BOOTSTRAP: Upgrading Bootstrapper..");

            var currentDirPath = currentDir.FullName;
            var origDir = new DirectoryInfo(currentDirPath.Substring(0, currentDirPath.Length - 8));
            var backupDirName = origDir.FullName + "_" + Guid.NewGuid().ToString().Replace("-", "");
            Directory.Move(origDir.FullName, backupDirName);
            Utilities.CopyAll(currentDir, origDir);

            File.WriteAllText(Path.Combine(currentDirPath, ".prime-upgraded"), "DONE");

            Console.WriteLine("BOOTSTRAP: Previous:\t" + origDir.FullName);
            Console.WriteLine("BOOTSTRAP: New:\t\t" + currentDirPath);
            Console.WriteLine("BOOTSTRAP: Backup:\t" + backupDirName);

            Console.WriteLine("BOOTSTRAP: Upgraded Bootstrapper.");
            Console.WriteLine("BOOTSTRAP: Starting: " + Path.Combine(origDir.FullName, execName) + " " + string.Join(" ", args));
            Process.Start(Path.Combine(origDir.FullName, execName), string.Join(" ", args));
            return true;
        }
    }
}