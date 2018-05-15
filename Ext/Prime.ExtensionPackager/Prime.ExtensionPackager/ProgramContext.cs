using System.IO;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class ProgramContext
    {
        public readonly ClientContext ClientContext;

        public ProgramContext(ClientContext clientContext)
        {
            ClientContext = clientContext;
        }

        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo StagingDirectory => ClientContext.FileSystem.StagingDirectory;

        public DirectoryInfo DistributionDirectory => ClientContext.FileSystem.DistributionDirectory;

        public bool IsBase { get; set; }

        public ObjectId ExtId { get; set; }

        public ILogger Logger { get; set; }
    }
}