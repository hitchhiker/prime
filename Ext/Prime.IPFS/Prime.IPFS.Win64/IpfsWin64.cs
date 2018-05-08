using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.IPFS.Win64
{
    public class IpfsWin64 : IpfsWin
    {
        public IpfsWin64(IExtension instance) : base(instance) {}

        public override string PackageInstallName => "go-ipfs_v0.4.14_windows-amd64.zip";
    }
}
