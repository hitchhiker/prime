using Prime.Core;
using Prime.PackageManager;

namespace Prime.Console.Frank
{
    public static class PackageTests
    {
        public static void PackageCoordinator(ClientContext context)
        {
            var pm = new PackageCoordinator(context);

            pm.EnsureInstalled();

            context.L.Info(pm.Distribution.Count);
        }

        public static void PackageCatalogue(ClientContext context)
        {
            var cbuild = new CatalogueBuilder(context);
            cbuild.Build();
        }
    }
}