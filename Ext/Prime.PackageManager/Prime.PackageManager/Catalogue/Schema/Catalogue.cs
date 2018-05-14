using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.PackageManager
{
    public class Catalogue
    {
        public Catalogue() { }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("catalogue")]
        public List<CataloguePackage> Entries { get; set; } = new List<CataloguePackage>();
    }
}