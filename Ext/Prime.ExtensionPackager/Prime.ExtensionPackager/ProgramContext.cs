using System.IO;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class ProgramContext
    {
        public readonly PrimeContext C;

        public ProgramContext(PrimeContext c)
        {
            C = c;
        }

        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo StagingDirectory => C.FileSystem.StagingDirectory;

        public DirectoryInfo DistributionDirectory => C.FileSystem.DistributionDirectory;

        public bool IsBase { get; set; }

        public ObjectId ExtId { get; set; }

        public ILogger Logger { get; set; }

        public bool ExtractNuget { get; set; }
    }
}