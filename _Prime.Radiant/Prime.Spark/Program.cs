using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Spark
{
    class Program
    {
        static void Main(string[] args)
        {
            var dd = GetDeployDir("radiant");

            DirectoryInfo founddeploy = null;

            foreach (var d in dd.GetDirectories().OrderByDescending(x => x.CreationTimeUtc))
            {
                if (!IsRetrieved(d))
                    continue;

                founddeploy = d;
                break;
            }

            if (founddeploy != null)
            {
                //cleanup deploy directory

                Trim(dd);

                /*
                //cleanup initial files;

                var initpath = GetInitialInstallPath();

                var isdebug = initpath.IndexOf("debug", StringComparison.OrdinalIgnoreCase) > -1 || initpath.IndexOf("release", StringComparison.OrdinalIgnoreCase) > -1;

                if (!isdebug)
                {
                    var files = new DirectoryInfo(GetInitialInstallPath()).GetFileSystemInfos("*", SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                        if (f.Name.IndexOf("spark", StringComparison.OrdinalIgnoreCase) > -1)
                            continue;
                        f.Delete();
                    }
                }*/

                var path = Path.Combine(Path.Combine(founddeploy.FullName, "bin"), Constants.RadiantExeName);
                BootApplication(path);
                return;
            }

            BootApplication(GetInitialInstall());
        }

        private static string GetInitialInstall()
        {
            var spark = new FileInfo(Path.Combine(GetInitialInstallPath(), Constants.RadiantExeName));
            return spark.FullName;
        }

        private static string GetInitialInstallPath()
        {
            var fileinfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var dirinfo = fileinfo.Directory;
            return dirinfo.FullName;
        }

        private static bool IsRetrieved(DirectoryInfo dir)
        {
            return new FileInfo(Path.Combine(dir.FullName, ".retrieved")).Exists;
        }

        private static DirectoryInfo GetDeployDir(string projectKey)
        {
            var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Prime"));
            if (!di.Exists)
                di.Create();

            var deployDirectory = new DirectoryInfo(Path.Combine(di.FullName, "deploy"));
            if (!deployDirectory.Exists)
                deployDirectory.Create();

            deployDirectory = new DirectoryInfo(Path.Combine(deployDirectory.FullName, projectKey));
            if (!deployDirectory.Exists)
                deployDirectory.Create();

            return deployDirectory;
        }

        public static void Trim(DirectoryInfo deployBaseDirectory, int retentionAmount = 4)
        {
            var dirPaths = Directory.EnumerateDirectories(deployBaseDirectory.FullName, "_*", new SearchOption()).ToList();

            if (dirPaths.Count <= retentionAmount)
                return;

            var dirs = dirPaths.Select(x => new DirectoryInfo(x)).ToList();
            var toRemove = dirs.Count - retentionAmount;

            foreach (var d in dirs.OrderBy(x => x.CreationTimeUtc).Take(toRemove))
                d.Delete(true);
        }

        private static void BootApplication(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
