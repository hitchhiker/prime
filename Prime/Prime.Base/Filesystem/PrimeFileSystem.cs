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
        public readonly DirectoryInfo PrimeWorkspaceDirectory;

        public PrimeFileSystem(PrimeContext context)
        {
            _context = context;
            PrimeWorkspaceDirectory = GetPrimeDirectory();
        }
        
        private DirectoryInfo _configDirectory;
        public DirectoryInfo ConfigDirectory => _configDirectory ?? (_configDirectory = GetSubDirectory("config"));

        private DirectoryInfo _workspaceDirectory;
        public DirectoryInfo WorkspaceDirectory => _workspaceDirectory ?? (_workspaceDirectory = GetSubDirectory("workspace"));

        private DirectoryInfo _tmpDirectory;
        public DirectoryInfo TmpDirectory => _tmpDirectory ?? (_tmpDirectory = GetSubDirectory("tmp"));

        private DirectoryInfo _usr;
        public DirectoryInfo UsrDirectory => _usr ?? (_usr = GetSubDirectory("usr"));

        private DirectoryInfo _apiConfigPath;
        public DirectoryInfo ApiConfigPath => _apiConfigPath ?? (_apiConfigPath = ConfigDirectory.EnsureSubDirectory("keys"));

        private DirectoryInfo _packageDirectory;
        public DirectoryInfo PackageDirectory => _packageDirectory ?? (_packageDirectory = GetSubDirectory("package"));

        private DirectoryInfo _distributionDirectory;
        public DirectoryInfo DistributionDirectory => _distributionDirectory ?? (_distributionDirectory = PackageDirectory.EnsureSubDirectory("dist"));

        private DirectoryInfo _installDirectory;
        public DirectoryInfo InstallDirectory => _installDirectory ?? (_installDirectory = PackageDirectory.EnsureSubDirectory("install"));

        private DirectoryInfo _stagingDirectory;
        public DirectoryInfo StagingDirectory => _stagingDirectory ?? (_stagingDirectory = TmpDirectory.EnsureSubDirectory("staging"));

        private DirectoryInfo _catDirectory;
        public DirectoryInfo CatalogueDirectory => _catDirectory ?? (_catDirectory = PackageDirectory.EnsureSubDirectory("catalogue"));

        private DirectoryInfo GetPrimeDirectory()
        {
            return _context.AppDataDirectoryInfo.EnsureSubDirectory(_context.Name);
        }

        public DirectoryInfo GetSubDirectory(string directoryName)
        {
            return PrimeWorkspaceDirectory.EnsureSubDirectory(directoryName);
        }

        public DirectoryInfo GetExtWorkspace(IExtension ext)
        {
            var ws = ext.Id.ToString();
            if (!(ext is IExtensionPlatform plt))
                return WorkspaceDirectory.EnsureSubDirectory(ws);

            if (plt.Platform!= Platform.NotSpecified)
                ws += "-" + plt.Platform.ToString().ToLower();
            return WorkspaceDirectory.EnsureSubDirectory(ws);
        }
    }
}
