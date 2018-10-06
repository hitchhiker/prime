using Prime.Core;
using System.Composition;
using Prime.Base;

namespace Prime.IPFS.Win64
{
    [Export(typeof(IExtension))]
    public class IpfsWin64Extension : IpfsWinExtensionBase
    {
        public override Platform Platform => Platform.WinAmd64;

        public override IpfsPlatformBase GetPlatformBase() => new IpfsPlatformWin64(this);
    }
}