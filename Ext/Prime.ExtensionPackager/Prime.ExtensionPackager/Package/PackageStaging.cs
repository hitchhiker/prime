using System;
using System.IO;
using System.Linq;

namespace Prime.ExtensionPackager
{
    public class PackageStaging
    {
        private readonly Package _package;
        private readonly ProgramContext _pars;

        public PackageStaging(Package package, ProgramContext pars)
        {
            _package = package;
            _pars = pars;
        }

        public void Stage()
        {
            var stageDir = new DirectoryInfo(Path.Combine(_pars.StagingDirectory.FullName, _package.GetDirectory()));

            if (stageDir.Exists)
                stageDir.Delete(true);

            stageDir.Create();

            var package = _package;
            var vpOffset = _pars.SourceDirectory.FullName.Length;

            foreach (var fi in package)
            {
                var vp = fi.FullName.Substring(vpOffset);
                var dst = new FileInfo(Path.Combine(stageDir.FullName, vp));

                if (!dst.Directory.Exists)
                    dst.Directory.Create();

                fi.CopyTo(dst.FullName, true);
            }

            package.MetaInfo.CopyTo(Path.Combine(stageDir.FullName, PackageMetaInspector.ExtFileName), true);

            Console.WriteLine("");
            Console.WriteLine($"{package.Count() + 1} file(s) copied to " + stageDir.FullName);
        }
    }
}