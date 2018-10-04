using System.IO;
using Prime.Base;
using Prime.Core;
using Prime.NetCoreExtensionPackager;
using Prime.NetCoreExtensionPackager.Compiler;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager
{
    public class PackageCompilationEntry : CommonBase
    {
        public PackageCompilationEntry(PrimeContext context) : base(context) { }

        public PackageCompilationEntry(CommonBase otherBase) : base(otherBase) { }

        public bool Compile(PackageConfig config)
        {
            foreach (var packageConfigItem in config.Packages)
            {
                if (!Compile(packageConfigItem))
                    return false;
            }

            return true;
        }

        public bool Compile(PackageConfigItem packageItem)
        {
            L.Log("Starting compilation for: " + packageItem.Source);

            if (packageItem.Type != "netcore")
            {
                L.Error("Unable to compile item of type: " + packageItem.Type + " - aborting.");
                return false;
            }

            var sourceDir = new DirectoryInfo(packageItem.Source.ResolveSpecial());
            var tmpPublishDir = C.FileSystem.GetTmpSubDirectory("publish");
            var destinationDir = new DirectoryInfo(Path.Combine(tmpPublishDir.FullName, sourceDir.Name));

            return CoreCompile(packageItem, sourceDir, destinationDir);
        }

        private bool CoreCompile(PackageConfigItem packageItem, DirectoryInfo sourceDir, DirectoryInfo destinationDir)
        {
            var success = NetCoreNativeCompiler.Compile(C, sourceDir.FullName, destinationDir.FullName);

            if (success)
                L.Log($"{packageItem.Source} compiled as 'Release' successfully.");
            else
                L.Error($"{packageItem.Source} failed to compile, aborting.");

            return success;
        }
    }
}