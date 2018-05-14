using System;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public static class Process
    {
        public static void Go(ProgramContext ctx)
        {
            var pmi = new PackageMetaInspector(ctx);
            pmi.Inspect();

            if (pmi.Package == null)
            {
                ctx.Logger.Info("No package created.");
                return;
            }

            var staging = new PackageStaging(pmi.Package, ctx);
            staging.Stage();

            var bundler = new PackageBundler(pmi.Package, ctx);
            bundler.Bundle();
        }
    }
}