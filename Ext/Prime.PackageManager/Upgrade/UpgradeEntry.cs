using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager.Upgrade
{
    public class UpgradeEntry : CommonBase
    {
        private readonly PrimeInstance _instance;
        private readonly PackageConfig _packageConfig;

        public UpgradeEntry(PrimeInstance instance, PackageConfig packageConfig) : base(instance)
        {
            _instance = instance;
            _packageConfig = packageConfig;
        }

        public bool Upgrade()
        {
            L.Log("Starting package upgrade..");
            var r = _instance.ExtensionManager.UpgradeInstallVersions();
            L.Log(r ? "Packages upgraded." : "No packages upgraded");
            return r;
        }
    }
}
