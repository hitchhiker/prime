using System;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    public static class NetCorePackager
    {
        public static bool PackageItem(NetCorePackagerContext ctx)
        {
            var pmi = new PackageMetaInspector(ctx);
            pmi.Inspect();

            if (pmi.Package == null)
            {
                ctx.L.Info("No package created.");
                return false;
            }

            var staging = new PackageStaging(pmi.Package, ctx);
            staging.Stage();

            var bundler = new PackageBundler(pmi.Package, ctx);
            bundler.Bundle();

            return true;
        }
    }
}