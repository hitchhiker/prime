using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Base;
using Prime.Core;

namespace Prime.IPFS.Win32
{
    public class IpfsPlatformWin32 : IpfsPlatformWin
    {
        public IpfsPlatformWin32(IExtension instance) : base(instance) {}

        public override string PackageInstallName => "go-ipfs_v0.4.17_windows-386.zip";
    }
}
