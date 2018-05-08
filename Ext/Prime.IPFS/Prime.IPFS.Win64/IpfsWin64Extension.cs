using Prime.Core;
using System.Composition;

namespace Prime.IPFS.Win64
{
    [Export(typeof(IExtensionExecute))]
    public class IpfsWin64Extension : IpfsExtension
    {
        public override Platform Platform => Platform.WinAmd64;

        public override IpfsPlatformBase GetPlatformBase() => new IpfsWin64(this);
    }
}