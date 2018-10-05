using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Prime.Base;

namespace Prime.Core.Extensions
{
    public class PrimeUpgrade : CommonBase
    {
        private readonly ExtensionManager _manager;
        private readonly InstallConfig _install;

        public PrimeUpgrade(ExtensionManager manager) : base(manager.Context)
        {
            _manager = manager;
            _install = _manager.InstallConfig;
        }

        public void CheckInstallVersions()
        {
            if (C.Config.ConfigLoadedFrom == null)
                return;

            var items = _install.Installs.Where(x => x.Version == null).ToList();

            var r = new PrimeUpgradeReport(items);

            foreach (var p in items)
                UpgradeInstallVersion(p, r);

            UpgradeCoreEntry(r);

            if (!r.IsUpgraded)
            {
                r.WriteReport(L);
                return;
            }

            L.Info("Updating main config with current versions for installed packages.");

            _manager.SaveConfig();

            r.WriteReport(L);
        }

        public PrimeUpgradeReport UpgradeInstallVersions()
        {
            var ic = _manager.InstallConfig;
            var items = ic.Installs.ToList();
            var r = new PrimeUpgradeReport(items);

            if (C.Config.ConfigLoadedFrom==null)
                return r;

            foreach (var p in items)
                UpgradeInstallVersion(p, r);

            UpgradeCoreEntry(r);

            if (!r.IsUpgraded)
            {
                r.WriteReport(L);
                return r;
            }

            L.Info("Updating main config with latest versions for installed packages.");

            _manager.SaveConfig();

            r.WriteReport(L);
            return r;
        }

        private void UpgradeInstallVersion(InstallEntry p, PrimeUpgradeReport r)
        {
            var dir = _manager.Instances.GetPackageDirectory(p.Id);
            if (dir == null)
            {
                r.ReportUnChanged(p.Id, "Unable to find package directory.");
                return;
            }

            var pm = PackageMeta.From(dir);
            if (pm == null)
            {
                r.ReportUnChanged(p.Id, $"Unable to find {PackageMeta.CommonName}");
                return;
            }

            if (p.Version!=null && p.Version >= pm.Version)
            {
                r.ReportUnChanged(p.Id, "Version is same or higher.");
                return;
            }

            r.ReportUpgrade(p.Id, pm.Title, pm.Version);

            p.VersionString = pm.Version.ToString();
            p.Title = pm.Title ?? p.Title;
        }

        private void UpgradeCoreEntry(PrimeUpgradeReport r)
        {
            var ic = _manager.InstallConfig;
            var coreUpdated = r.FirstOrDefault(x => x.Id == PrimeCoreExtension.StaticId);
            if (coreUpdated == null)
                return;

            if (ic.EntryPointCore != null && ic.EntryPointCore == coreUpdated.LatestVersion?.ToString())
                return;

            if (coreUpdated.UpgradedVersion == null)
                return;

            ic.EntryPointCore = coreUpdated.UpgradedVersion.ToString();
            r.CoreEntryUpdated = ic.EntryPointCore;
        }
    }
}
