using System.IO;
using Prime.Core;

namespace Prime.IPFS
{
    public class IpfsInstanceContext
    {
        public IpfsInstanceContext(DirectoryInfo workspaceDirectory, IpfsPlatformBase platform)
        {
            WorkspaceDirectory = workspaceDirectory;
            Platform = platform;
        }

        public DirectoryInfo WorkspaceDirectory { get; private set; }

        public IpfsPlatformBase Platform { get; private set; }

        public ILogger Logger { get; set; }
    }
}