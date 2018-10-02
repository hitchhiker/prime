using Prime.Base;
using Prime.Core;

namespace Prime.IPFS
{
    public class IpfsPlatformGeneric : IpfsPlatformBase
    {
        public IpfsPlatformGeneric(IExtension instance) : base(instance) { }

        public override string PackageInstallName => "System IPFS";

        public override string NativeExecutable => "generic";

        public override IpfsDaemonBase GetDaemon(IpfsInstance instance)
        {
            return new IpfsGenericDaemon(instance);
        }

        public override void Install(IpfsInstance instance)
        {
            //
        }
    }
}