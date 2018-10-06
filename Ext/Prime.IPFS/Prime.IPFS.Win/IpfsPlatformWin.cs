using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Base;
using Prime.Core;
using Prime.Core.Windows;

namespace Prime.IPFS
{
    public abstract class IpfsPlatformWin : IpfsPlatformBase
    {
        protected IpfsPlatformWin(IExtension instance) : base(instance) {} 

        public override string NativeExecutable => "ipfs.exe";

        public override IpfsDaemonBase GetDaemon(IpfsInstance instance)
        {
            return new IpfsWindowsDaemon(instance);
        }

        public override void Install(IpfsInstance instance)
        {
            if (!PackageInstallName.EndsWith(".zip"))
                throw new Exception("The package specified '" + PackageInstallName + "' is not the expected .zip file.");

            var zipInfo = new FileInfo(Path.Combine(instance.ExecutingDirectory.FullName, PackageInstallName));
            if (!zipInfo.Exists)
                throw new Exception(zipInfo.FullName + " does not exist, unable to install IPFS for Windows.");

            var tmp = instance.TempDirectory.EnsureTempSubDirectory();
            
            System.IO.Compression.ZipFile.ExtractToDirectory(zipInfo.FullName, tmp.FullName);

            var exe = new FileInfo(Path.Combine(tmp.FullName, Path.Combine("go-ipfs", NativeExecutable)));
            if (!exe.Exists)
                throw new Exception(exe.FullName + " does not exist, package archive has unexpected structure. Unable to install IPFS for Windows.");

            File.Copy(exe.FullName, instance.NativeExecutable.FullName);
            instance.NativeExecutable.Refresh();

            tmp.Delete(true);

            try
            {
                FirewallHelper.Instance.GrantAuthorization(instance.NativeExecutable.FullName, "prime-ipfs");
            }
            catch
            {
                instance.L.Warn("Unable to modify the Windows Firewall for IPFS. Please do this manually.");
            }
        }
    }
}
