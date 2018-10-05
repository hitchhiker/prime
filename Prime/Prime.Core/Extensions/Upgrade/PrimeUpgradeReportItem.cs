using System;
using Prime.Base;

namespace Prime.Core.Extensions
{
    public class PrimeUpgradeReportItem
    {
        public ObjectId Id { get; set; }
        public string ExtTitle { get; set; }
        public Version PreviousVersion { get; set; }
        public Version UpgradedVersion { get; set; }
        public string UnchangedReason { get; set; }

        public Version LatestVersion => UpgradedVersion != null ? UpgradedVersion : PreviousVersion;
    }
}