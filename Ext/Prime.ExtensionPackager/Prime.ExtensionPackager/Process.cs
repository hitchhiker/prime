using System;

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
                Console.WriteLine("No package found.");
                return;
            }

            var stageing = new PackageStaging(pmi.Package, ctx);
            stageing.Stage();
        }
    }
}