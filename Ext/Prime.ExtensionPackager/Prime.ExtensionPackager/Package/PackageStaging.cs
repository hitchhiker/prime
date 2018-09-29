using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class PackageStaging
    {
        private readonly Package _package;
        public readonly ProgramContext Context;

        public PackageStaging(Package package, ProgramContext context)
        {
            _package = package;
            Context = context;
        }

        public void Stage()
        {
            var stageDir = new DirectoryInfo(Path.Combine(Context.StagingDirectory.FullName, _package.GetDirectory()));

            if (stageDir.Exists)
                stageDir.Delete(true);

            stageDir.Create();

            var package = _package;
            var vpOffset = Context.SourceDirectory.FullName.Length;

            var staged = new List<FileInfo>();

            foreach (var fi in package)
            {
                //if (!Context.IsBase && fi.Name.StartsWith("prime.core.", StringComparison.OrdinalIgnoreCase))
                //    continue;

                var vp = fi.FullName.Substring(vpOffset);

                if (vp.StartsWith(Path.DirectorySeparatorChar))
                    vp = vp.Substring(1);

                var dst = new FileInfo(Path.Combine(stageDir.FullName, vp));

                if (!dst.Directory.Exists)
                    dst.Directory.Create();

                fi.CopyTo(dst.FullName, true);
                staged.Add(dst);
            }

            var sMetaPath = Path.Combine(stageDir.FullName, PackageMetaInspector.ExtFileName);
            package.MetaInfo.CopyTo(sMetaPath, true);

            package.AddStaged(stageDir, staged, new FileInfo(sMetaPath));

            Context.Logger.Info("");
            Context.Logger.Info($"{package.Count() + 1} file(s) copied to " + stageDir.FullName);

            if (Context.ExtractNuget)
            {
                var nuget = new NugetExtractor(Context, package);
                nuget.Extract(stageDir);
            }
        }
    }
}