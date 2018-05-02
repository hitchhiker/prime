using System.IO;

namespace Prime.IPFS
{
    public abstract class IpfsPlatformBase
    {
        public abstract string PackageInstallName { get; }

        public abstract string NativeExecutable { get; }

        public abstract IpfsDaemonBase GetDaemon(IpfsInstance instance);
        
        public abstract void Install(IpfsInstance instance);
    }
}