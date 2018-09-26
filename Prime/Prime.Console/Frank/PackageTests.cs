using Prime.Core;
using Prime.PackageManager;

namespace Prime.Console.Frank
{
    public static class PackageTests
    {
        public static void PackageCoordinator(PrimeContext context)
        {
            //var pm = new PackageCoordinator(context);

            //pm.EnsureInstalled();

            //context.L.Info(pm.Distribution.Count);
        }

        public static void PackageCatalogue(PrimeInstance prime)
        {
            prime.Start();
            var cbuild = new CatalogueBuilder(prime);
            cbuild.Build();
        }
    }
}