using System.IO;

namespace Prime.ExtensionPackager
{
    public class ProgramContext
    {
        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo StagingDirectory { get; set; }

        public DirectoryInfo DistributionDirectory { get; set; }

        public bool IsPrime { get; set; }
    }
}