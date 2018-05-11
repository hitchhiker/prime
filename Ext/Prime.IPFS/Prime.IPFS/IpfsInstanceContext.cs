using System.IO;
using Prime.Core;

namespace Prime.IPFS
{
    public class IpfsInstanceContext
    {
        public readonly ServerContext ServerContext;

        public IpfsInstanceContext(ServerContext context, IpfsPlatformBase platform)
        {
            ServerContext = context;
            WorkspaceDirectory = context.FileSystem.GetExtWorkspace(platform.Instance);
            Platform = platform;
        }

        public DirectoryInfo WorkspaceDirectory { get; private set; }

        public IpfsPlatformBase Platform { get; private set; }

        public ILogger Logger { get; set; }
    }
}