using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Base;

namespace Prime.Core.Extensions
{
    public class PrimeUpgradeReport : List<PrimeUpgradeReportItem>
    {
        public PrimeUpgradeReport(IEnumerable<InstallEntry> entries)
        {
            foreach (var i in entries)
                Add(new PrimeUpgradeReportItem() {Id = i.Id, PreviousVersion = i.Version, ExtTitle = i.Title});
        }

        public bool IsUpgraded => CoreEntryUpdated!=null || this.Any(x => x.UpgradedVersion != null);

        public void ReportUpgrade(ObjectId id, string title, Version version)
        {
            var item = this.First(x => x.Id == id);
            item.UpgradedVersion = version;
            item.ExtTitle = title ?? item.ExtTitle;
        }

        public void ReportUnChanged(ObjectId id, string reason)
        {
            var item = this.First(x => x.Id == id);
            item.UnchangedReason = reason;
        }

        public void WriteReport(ILogger l)
        {
            foreach (var i in this.Where(x=>x.UpgradedVersion == null))
                l.Info($"Extension '{i.ExtTitle ?? "Unknown"}' {i.PreviousVersion} unchanged '" + i.UnchangedReason + "'");

            foreach (var i in this.Where(x => x.UpgradedVersion != null))
                l.Info($"Extension '{i.ExtTitle ?? "Unknown"}' {i.PreviousVersion} upgraded to {i.UpgradedVersion}");

            if (CoreEntryUpdated!=null)
                l.Info($"Boostrap entry point set to {CoreEntryUpdated}");
        }

        public string CoreEntryUpdated { get; set; }
    }
}
