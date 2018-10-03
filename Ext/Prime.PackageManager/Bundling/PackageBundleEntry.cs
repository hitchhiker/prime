using System.IO;
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

        public bool Bundle(PackageConfig config)
        {
            foreach (var packageConfigItem in config.Packages)
            {
                if (!Bundle(packageConfigItem))
                    return false;
            }

            return true;
        }

        public bool Bundle(PackageConfigItem packageItem)
        {
            L.Log("Starting bundling for: " + packageItem.Source);

            if (packageItem.Type != "netcore")
            {
                L.Error("Unable to compile item of type: " + packageItem.Type + " - aborting.");
                return false;
            }

            var sourceDir = new DirectoryInfo(packageItem.Source.ResolveSpecial());
            var tmpPublishDir = C.FileSystem.GetTmpSubDirectory("publish");
            var destinationDir = new DirectoryInfo(Path.Combine(tmpPublishDir.FullName, sourceDir.Name));

            return Bundle(packageItem, destinationDir);
        }


        private bool Bundle(PackageConfigItem packageItem, DirectoryInfo destinationDir)
        {
            var ctx = new NetCorePackagerContext(C) { SourceDirectory = destinationDir, ExtId = packageItem.GetId() };
            var packageSuccess = NetCorePackager.PackageItem(ctx);

            if (packageSuccess)
                L.Log($"{packageItem.Source} bundled successfully.");
            else
                L.Error($"{packageItem.Source} failed.");

            L.Log("");
            return packageSuccess;
        }
    }
}