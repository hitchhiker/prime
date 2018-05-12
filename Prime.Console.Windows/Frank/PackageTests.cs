using Prime.Core;
using Prime.PackageManager;

namespace Prime.ConsoleApp.Tests.Frank
{
    public static class PackageTests
    {
        public static void PackageCoordinator(ServerContext context)
        {
            // BUG: AY: PackageCoordinator is not found.

            //var pm = new PackageCoordinator(context);

            //pm.EnsureInstalled();

            //context.L.Info(pm.Distribution.Count);
        }

        public static void PackageCatalogue(ServerContext context)
        {
            var cbuild = new CatalogueBuilder(context);
            cbuild.Build();
        }
    }
}