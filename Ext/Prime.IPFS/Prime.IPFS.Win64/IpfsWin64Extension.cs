using Prime.Core;
using System.Composition;

namespace Prime.IPFS.Win64
{
    [Export(typeof(IExtension))]
    public class IpfsWin64Extension : IpfsExtensionBase
    {
        public override Platform Platform => Platform.WinAmd64;

        public override IpfsPlatformBase GetPlatformBase() => new IpfsPlatformWin64(this);
    }
}