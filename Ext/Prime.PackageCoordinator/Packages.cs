using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Prime.Core;

namespace Prime.Core
{
    public class Packages : UniqueList<PackageContainer>
    {
        public readonly PackageCoordinator Coordinator;
        public readonly DirectoryInfo BaseDirectory;
        public readonly PrimeContext Context;

        public Packages(PackageCoordinator coordinator, DirectoryInfo baseDirectory)
        {
            Coordinator = coordinator;
            BaseDirectory = baseDirectory;
            Context = coordinator.Context;
            foreach (var d in baseDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                var pc = new PackageContainer(this, d);
                if (!pc.IsEmpty)
                    base.Add(pc);
            }
        }
    }
}