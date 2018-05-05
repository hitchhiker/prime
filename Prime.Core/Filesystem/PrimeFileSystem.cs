using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public class PrimeFileSystem
    {
        private readonly PrimeContext _context;
        public readonly DirectoryInfo PrimeDirectory;

        public PrimeFileSystem(PrimeContext context)
        {
            _context = context;
            PrimeDirectory = GetPrimeDirectory();
        }
        
        private DirectoryInfo _configDirectory;
        public DirectoryInfo ConfigDirectory => _configDirectory ?? (_configDirectory = GetSubDirectory("config"));

        private DirectoryInfo _workspaceDirectory;
        public DirectoryInfo WorkspaceDirectory => _workspaceDirectory ?? (_workspaceDirectory = GetSubDirectory("workspace"));

        private DirectoryInfo _tmpDirectory;
        public DirectoryInfo TmpDirectory => _tmpDirectory ?? (_tmpDirectory = GetSubDirectory("tmp"));

        private DirectoryInfo _packageDirectory;
        public DirectoryInfo PackageDirectory => _packageDirectory ?? (_packageDirectory = GetSubDirectory("package"));

        private DirectoryInfo _usr;
        public DirectoryInfo UsrDirectory => _usr ?? (_usr = GetSubDirectory("usr"));

        private DirectoryInfo _stagingDirectory;
        public DirectoryInfo StagingDirectory => _stagingDirectory ?? (_stagingDirectory = TmpDirectory.EnsureSubDirectory("staging"));

        private DirectoryInfo _catDirectory;
        public DirectoryInfo CatalogueDirectory => _catDirectory ?? (_catDirectory = WorkspaceDirectory.EnsureSubDirectory("catalogue"));

        private DirectoryInfo _apiConfigPath;
        public DirectoryInfo ApiConfigPath => _apiConfigPath ?? (_apiConfigPath = ConfigDirectory.EnsureSubDirectory("keys"));

        private DirectoryInfo GetPrimeDirectory()
        {
            return _context.WorkContainerDirectoryInfo.EnsureSubDirectory("prime");
        }

        public DirectoryInfo GetSubDirectory(string directoryName)
        {
            return PrimeDirectory.EnsureSubDirectory(directoryName);
        }
    }
}
