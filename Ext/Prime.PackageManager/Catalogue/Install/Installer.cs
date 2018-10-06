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
                var hasPlat = p.PackageInstances.Any(x => x.Platform != Platform.NotSpecified);
                var insts = hasPlat
                    ? p.PackageInstances.Where(x => x.Platform == C.PlatformCurrent).ToList()
                    : p.PackageInstances;

                var v = insts.Max(x => x.Version);
                foreach (var i in insts.Where(x=>x.Version == v))
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
            if (!response.Success)
            {
                L.Error("Unable to download: " + package.ContentUri.Path);
                return;
            }

            L.Info("Downloaded.");
            L.Info("Unpacking..");

            foreach (var i in new DirectoryInfo(distPath).GetFiles("arc.bz2.*"))
            {
                if (i.Name.StartsWith("arc.bz2"))
                    Decompression.ExtractArchive(i, installPath);
            }

            var pm = Path.Combine(distPath, PackageMeta.CommonName);
            if (File.Exists(pm))
                File.Copy(Path.Combine(distPath, PackageMeta.CommonName), Path.Combine(installPath, PackageMeta.CommonName), true);

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
