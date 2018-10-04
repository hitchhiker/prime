using System.IO;
using Prime.Base;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    public class NetCorePackagerContext : CommonBase
    {
        public NetCorePackagerContext(PrimeContext c) : base(c)
        {
        }

        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo StagingDirectory => C.FileSystem.StagingDirectory;

        public DirectoryInfo DistributionDirectory => C.FileSystem.DistributionDirectory;

        public bool IsBase => ExtId == new ObjectId("ded1293613356d85c37e509a");

        public ObjectId ExtId { get; set; }

        public bool ExtractNuget { get; set; }
    }
}