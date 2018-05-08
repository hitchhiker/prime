using System.IO;
using Prime.Core;

namespace Prime.IPFS
{
    public abstract class IpfsPlatformBase
    {
        public readonly IExtension Instance;

        protected IpfsPlatformBase(IExtension ext)
        {
            Instance = ext;
        }

        public abstract string PackageInstallName { get; }

        public abstract string NativeExecutable { get; }

        public abstract IpfsDaemonBase GetDaemon(IpfsInstance instance);
        
        public abstract void Install(IpfsInstance instance);
    }
}