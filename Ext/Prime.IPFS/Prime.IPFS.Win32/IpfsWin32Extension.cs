using System.Composition;
using Prime.Base;
using Prime.Core;

namespace Prime.IPFS.Win32
{
    [Export(typeof(IExtensionExecute))]
    public class IpfsWin32Extension : IpfsWinExtensionBase
    {
        public override Platform Platform => Platform.Win386;

        public override IpfsPlatformBase GetPlatformBase() => new IpfsPlatformWin32(this);
    }
}