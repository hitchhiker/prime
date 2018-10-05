using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Core;
using Prime.Core.Extensions;
using Prime.Radiant;
using PackageConfig = Prime.Radiant.PackageConfig;

namespace Prime.PackageManager.Upgrade
{
    public class UpgradeEntry : CommonBase
    {
        private readonly PrimeInstance _instance;

        public UpgradeEntry(PrimeInstance instance) : base(instance)
        {
            _instance = instance;
        }

        public void Upgrade()
        {
            L.Log("Starting package upgrade..");
            var r = new PrimeUpgrade(_instance.ExtensionManager).UpgradeInstallVersions();
            L.Log(r.IsUpgraded ? "Packages upgrade complete." : "No packages upgraded.");
        }
    }
}
