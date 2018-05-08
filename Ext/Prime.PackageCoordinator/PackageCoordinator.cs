using Prime.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Base;

namespace Prime.Core
{
    public class PackageCoordinator
    {
        public readonly PrimeContext Context;
        public readonly DirectoryInfo PackagesDir;
        public readonly Packages Distribution;
        public readonly Packages Installed;
        public InstallConfig InstallConfig => Context.PrimeConfig.PackageConfig.InstallConfig;

        public static string PrimeExtName = "prime-ext.json";
        public static readonly string ArchiveName = "arc.bz2";

        public PackageCoordinator(PrimeContext context)
        {
            Context = context;
            PackagesDir = context.FileSystem.PackageDirectory;
            Distribution = new Packages(this, context.FileSystem.DistributionDirectory);
            Installed = new Packages(this, context.FileSystem.InstallDirectory);
        }

        public PackageReport GenerateReport()
        {
            var installIds = InstallConfig.Installs;
            var remaining = installIds.Where(x => Installed.All(a => a.Id != x.Id)).ToList();
            var ready = remaining.Where(x => Distribution.Any(a => a.Id == x.Id)).ToList();
            var requireDist = remaining.Except(ready);

            var pr = new PackageReport()
            {
                Required = InstallConfig.Installs.Select(x => x.Id).ToUniqueList(),
                Distributed = Distribution.SelectMany(x => x).ToUniqueList(),
                Installed = Installed.SelectMany(x => x).ToUniqueList(),
                RequiresInstallOnly = ready.Select(x=>x.Id).ToUniqueList(),
                RequiresDistribution = requireDist.Select(x=>x.Id).ToUniqueList()
            };

            return pr;
        }

        public void EnsureInstalled()
        {
            var pr = GenerateReport();
            Install(pr.RequiresInstallOnly);
        }

        public void Install(IList<ObjectId> ids)
        {
            foreach (var i in Distribution.Where(x => ids.Any(e => e == x.Id)))
                i.Install();
        }
    }
}
