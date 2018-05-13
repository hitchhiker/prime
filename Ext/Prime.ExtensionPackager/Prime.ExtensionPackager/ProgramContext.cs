using System.IO;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class ProgramContext
    {
        private readonly ClientContext _context;

        public ProgramContext(ClientContext context)
        {
            _context = context;
        }

        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo StagingDirectory => _context.FileSystem.StagingDirectory;

        public DirectoryInfo DistributionDirectory => _context.FileSystem.DistributionDirectory;

        public bool IsPrime { get; set; }

        public ObjectId ExtId { get; set; }

        public ILogger Logger { get; set; }
    }
}