using Prime.Base.DStore;
using Prime.Core;
using Prime.Radiant;

namespace Prime.PackageManager
{
    public class PackageManagerEntry
    {
        public static ContentUri RequestBuild(PrimeInstance prime, PackageManagerArguments.BuildArguments arguments)
        {
            var config = RadiantEntry.GetPublisherConfig(prime, arguments.PubConfigPath);
            if (config == null)
                return new ContentUri();

            var packageBuilder = new PackageCatalogueBuilder(prime, config);
            return packageBuilder.Build();
        }

        public static void RequestCompile(PrimeInstance prime, PackageManagerArguments.CompileArguments arguments)
        { 
        }
    }
}
