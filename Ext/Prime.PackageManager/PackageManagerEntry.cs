using System.Reflection;
using Prime.Base.DStore;
using Prime.Core;
using Prime.PackageManager.Upgrade;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager
{
    public static class PackageManagerEntry
    {
        public static void RequestPublish(PrimeInstance prime, PackageManagerArguments.PublishArguments arguments)
        {
            var packConfig = PackageConfig.Get(prime, arguments.PackageConfigPath);
            if (packConfig == null)
                return;

            var pubConfig = CataloguePublisherConfig.Get(prime, arguments.PubConfigPath);
            if (pubConfig == null)
                return;

            var work1 = new PackageCompilationEntry(prime);
            if (!work1.Compile(packConfig))
                return;

            var work2 = new PackageBundleEntry(prime);
            if (!work2.Bundle(packConfig))
                return;

            var work3 = new PackageCatalogueEntry(prime, pubConfig);
            if (work3.Build().Path==null)
                return;
            
            RadiantEntry.Publish(prime, pubConfig);
        }

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

        public static void RequestUpgrade(PrimeInstance prime, PackageManagerArguments.UpgradeArguments arguments)
        {
            var config = PackageConfig.Get(prime, arguments.PackageConfigPath);
            if (config == null)
                return;

            var work = new UpgradeEntry(prime, config);
            work.Upgrade();
        }
    }
}
