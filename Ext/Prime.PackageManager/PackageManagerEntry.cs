using System.Reflection;
using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager
{
    public static class PackageManagerEntry
    {
        public static void RequestCompile(PrimeInstance prime, PackageManagerArguments.CompileArguments arguments)
        {
            var config = PackageConfig.Get(prime, arguments.PackageConfigPath);
            if (config == null)
                return;

            var work = new PackageCompilationEntry(prime);
            work.Compile(config);
        }

        public static void RequestBundle(PrimeInstance prime, PackageManagerArguments.BundleArguments arguments)
        {
            var config = PackageConfig.Get(prime, arguments.PackageConfigPath);
            if (config == null)
                return;

            var work = new PackageBundleEntry(prime);
            work.Bundle(config);
        }

        public static ContentUri RequestBuild(PrimeInstance prime, PackageManagerArguments.BuildArguments arguments)
        {
            var config = CataloguePublisherConfig.Get(prime, arguments.PubConfigPath);
            if (config == null)
                return new ContentUri();

            var work = new PackageCatalogueEntry(prime, config);
            return work.Build();
        }
    }
}
