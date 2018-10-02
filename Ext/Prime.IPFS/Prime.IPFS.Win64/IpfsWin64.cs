using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Base;
using Prime.Core;

namespace Prime.IPFS.Win64
{
    public class IpfsPlatformWin64 : IpfsPlatformWin
    {
        public IpfsPlatformWin64(IExtension instance) : base(instance) {}

        public override string PackageInstallName => "go-ipfs_v0.4.17_windows-amd64.zip";
    }
}
