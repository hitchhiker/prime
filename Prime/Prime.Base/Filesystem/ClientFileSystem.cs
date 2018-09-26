using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public class ClientFileSystem
    {
        private readonly ClientContext _context;
        public readonly DirectoryInfo PrimeWorkspaceDirectory;

        public ClientFileSystem(ClientContext context)
        {
            _context = context;
            PrimeWorkspaceDirectory = GetPrimeDirectory();
        }

        private DirectoryInfo GetPrimeDirectory()
        {
            return _context.AppDataDirectoryInfo.EnsureSubDirectory("prime-client");
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

            if (plt.Platform != Platform.NotSpecified)
                ws += "-" + plt.Platform.ToString().ToLower();
            return WorkspaceDirectory.EnsureSubDirectory(ws);
        }
    }
}
