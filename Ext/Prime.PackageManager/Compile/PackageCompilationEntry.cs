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
    public class PackageCompilationEntry : CommonBase
    {
        public PackageCompilationEntry(PrimeContext context) : base(context) { }

        public PackageCompilationEntry(CommonBase otherBase) : base(otherBase) { }

        public bool Compile(PackageConfig packageConfig)
        {
            foreach (var packageConfigItem in packageConfig.Packages.Where(x => !x.IsSuspended))
            {
                if (!Compile(packageConfigItem))
                    return false;
            }

            return true;
        }

        public bool Compile(PackageConfigItem packageConfigItem)
        {
            if (packageConfigItem.IsSuspended)
            {
                L.Log("Package is suspended, aborting.");
                return false;
            }

            L.Log("Starting compilation for: " + packageConfigItem.Source);

            if (packageConfigItem.Type != "netcore")
            {
                L.Error("Unable to compile item of type: " + packageConfigItem.Type + " - aborting.");
                return false;
            }

            var sourceDir = new DirectoryInfo(packageConfigItem.Source.ResolveSpecial());
            var tmpPublishDir = C.FileSystem.GetTmpSubDirectory("publish");
            var destinationDir = new DirectoryInfo(Path.Combine(tmpPublishDir.FullName, sourceDir.Name));

            return CoreCompile(packageConfigItem, sourceDir, destinationDir);
        }

        private bool CoreCompile(PackageConfigItem packageConfigItem, DirectoryInfo sourceDir, DirectoryInfo destinationDir)
        {
            var success = NetCoreNativeCompiler.Compile(C, sourceDir.FullName, destinationDir.FullName);

            if (success)
                L.Log($"{packageConfigItem.Source} compiled as 'Release' successfully.");
            else
                L.Error($"{packageConfigItem.Source} failed to compile, aborting.");

            return success;
        }
    }
}