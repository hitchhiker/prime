using System.IO;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class ProgramContext
    {
        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo WorkspaceDirectory { get; set; }

        private DirectoryInfo _stagingDirectory;
        public DirectoryInfo StagingDirectory => _stagingDirectory ?? (_stagingDirectory = WorkspaceDirectory.EnsureSubDirectory("stage"));

        private DirectoryInfo _distributionDirectory;
        public DirectoryInfo DistributionDirectory => _distributionDirectory ?? (_distributionDirectory = WorkspaceDirectory.EnsureSubDirectory("dist"));

        public bool IsPrime { get; set; }

        public ILogger Logger { get; set; }
    }
}