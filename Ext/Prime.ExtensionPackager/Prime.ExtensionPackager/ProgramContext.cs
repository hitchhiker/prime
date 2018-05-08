using System.IO;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class ProgramContext
    {
        private readonly PrimeContext _context;

        public ProgramContext(PrimeContext context)
        {
            _context = context;
        }

        public DirectoryInfo SourceDirectory { get; set; }

        public DirectoryInfo StagingDirectory => _context.FileSystem.StagingDirectory;

        public DirectoryInfo DistributionDirectory => _context.FileSystem.DistributionDirectory;

        public bool IsPrime { get; set; }

        public ILogger Logger { get; set; }
    }
}