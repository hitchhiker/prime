using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Prime.Base.DStore;
using Prime.Radiant;

namespace Prime.PackageManager
{
    public class PackageCatalogue : ICatalogue
    {
        public PackageCatalogue() { }

        public string CatalogueTypeName => "packages";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("packages")]
        public List<PackageEntry> Packages { get; set; } = new List<PackageEntry>();

        public CatalogueBuildInformation BuildInformation { get; set; }

        public List<ContentUri> AllContentUri()
        {
            return Packages.SelectMany(x => x.PackageInstances.Select(i => i.ContentUri)).ToList();
        }
    }
}