using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.PackageManager.Catalogue
{
    public class Installer : CommonBase
    {
        private readonly PrimeInstance _instance;
        private readonly PackageCatalogue _cat;

        public Installer(PrimeInstance instance, PackageCatalogue catalogue) : base(instance.C)
        {
            _instance = instance;
            _cat = catalogue;
        }

        public bool Install()
        {
            var ids = C.Config.ConfigPackageNode.InstallConfig.Installs.Select(x => x.Id).ToUniqueList();
            var packages = _cat.Packages.Where(x => ids.Contains(x.Id)).ToList();

            L.Info($"Installing {packages.Count} packages.");

            foreach (var p in packages)
            {
                var v = p.PackageInstances.Max(x => x.Version);
                foreach (var i in p.PackageInstances.Where(x=>x.Version == v))
                {
                    Install(p, i);
                }
            }
            return true;
        }

        public void Install(PackageEntry entry, PackageInstance package)
        {
            var dist = C.FileSystem.DistributionDirectory;
            var distPath = Path.Combine(dist.FullName, GetDirectory(entry, package));

            var install = C.FileSystem.InstallDirectory;
            var installPath = Path.Combine(install.FullName, GetDirectory(entry, package));

            if (Directory.Exists(installPath))
            {
                L.Info(installPath + " already exists, skipping.");
                return;
            }

            L.Info($"Downloading package '{entry.Title}' to {distPath}");

            var response = M.SendAndWait<GetContentRequest, GetContentResponse>(new GetContentRequest(distPath, package.ContentUri.Path) {IsDirectory = true});
            L.Info("Downloaded.");
            L.Info("Unpacking..");

            foreach (var i in new DirectoryInfo(distPath).GetFiles("arc.bz2.*"))
            {
                if (i.Name.StartsWith("arc.bz2"))
                    Decompression.ExtractArchive(i, installPath);
            }
            L.Info("Unpacked to " + installPath);
        }

        public string GetDirectory(PackageEntry entry, PackageInstance package)
        {
            var fsTitle = entry.Title.Replace(" ", "-");
            var all = package.Platform == Platform.NotSpecified;
            if (!all)
                return (fsTitle + "-" + entry.Id + Path.DirectorySeparatorChar + package.Version + "-" + package.Platform).ToLower() + Path.DirectorySeparatorChar;

            return (fsTitle + "-" + entry.Id + Path.DirectorySeparatorChar + package.Version).ToLower() + Path.DirectorySeparatorChar;
        }
    }
}
