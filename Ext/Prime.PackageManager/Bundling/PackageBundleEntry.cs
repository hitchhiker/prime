using System.IO;
using System.Linq;
using Prime.Base;
using Prime.Core;
using Prime.NetCoreExtensionPackager;
using Prime.NetCoreExtensionPackager.Compiler;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager
{
    public class PackageBundleEntry : CommonBase
    {
        public PackageBundleEntry(PrimeContext context) : base(context) { }

        public PackageBundleEntry(CommonBase otherBase) : base(otherBase) { }

        public bool Bundle(PackageConfig packageConfig)
        {
            foreach (var packageConfigItem in packageConfig.Packages.Where(x=>!x.IsSuspended))
            {
                if (!Bundle(packageConfigItem))
                    return false;
            }

            return true;
        }

        public bool Bundle(PackageConfigItem packageConfigItem)
        {
            if (packageConfigItem.IsSuspended)
            {
                L.Log("Package is suspended, aborting.");
                return false;
            }

            L.Log("Starting bundling for: " + packageConfigItem.Source);

            if (packageConfigItem.Type != "netcore")
            {
                L.Error("Unable to compile item of type: " + packageConfigItem.Type + " - aborting.");
                return false;
            }

            var sourceDir = new DirectoryInfo(packageConfigItem.Source.ResolveSpecial());
            var tmpPublishDir = C.FileSystem.GetTmpSubDirectory("publish");
            var destinationDir = new DirectoryInfo(Path.Combine(tmpPublishDir.FullName, sourceDir.Name));

            return Bundle(packageConfigItem, destinationDir);
        }


        private bool Bundle(PackageConfigItem packageConfigItem, DirectoryInfo destinationDir)
        {
            var ctx = new NetCorePackagerContext(C) { SourceDirectory = destinationDir, ExtId = packageConfigItem.GetId() };
            var packageSuccess = NetCorePackager.PackageItem(ctx);

            if (packageSuccess)
                L.Log($"{packageConfigItem.Source} bundled successfully.");
            else
                L.Error($"{packageConfigItem.Source} failed.");

            L.Log("");
            return packageSuccess;
        }
    }
}