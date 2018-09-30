using System.Composition;
using Prime.Core;

namespace Prime.IPFS.Lin64
{
    [Export(typeof(IExtensionExecute))]
    public class IpfsLinux64Extension : IpfsExtensionBase
    {
        public override Platform Platform => Platform.LinuxAmd64;

        public override IpfsPlatformBase GetPlatformBase() => new IpfsPlatformGeneric(this);
    }
}