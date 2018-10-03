using System.Reflection;
using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager
{
    public static class PackageManagerEntry
    {
        public static ContentUri RequestBuild(PrimeInstance prime, PackageManagerArguments.BuildArguments arguments)
        {
            var config = CataloguePublisherConfig.Get(prime, arguments.PubConfigPath);
            if (config == null)
                return new ContentUri();

            var packageBuilder = new PackageCatalogueBuilder(prime, config);
            return packageBuilder.Build();
        }

        public static void RequestCompile(PrimeInstance prime, PackageManagerArguments.CompileArguments arguments)
        {
            var config = PackageConfig.Get(prime, arguments.PackageConfigPath);
            if (config == null)
                return;

            var comp = new PackageCompilation(prime);
            comp.Compile(config);
        }
    }
}
