using System.Collections.Generic;
using System.IO;

namespace Prime.Core
{
    public class Packages : UniqueList<Package>
    {
        public readonly PackageCoordinator Coordinator;
        public readonly DirectoryInfo BaseDirectory;
        public readonly PrimeContext Context;

        public Packages(PackageCoordinator coordinator, DirectoryInfo baseDirectory)
        {
            Coordinator = coordinator;
            BaseDirectory = baseDirectory;
            Context = coordinator.C;
            foreach (var d in baseDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                var pc = new Package(this, d);
                if (!pc.IsEmpty)
                    base.Add(pc);
            }
        }
    }
}