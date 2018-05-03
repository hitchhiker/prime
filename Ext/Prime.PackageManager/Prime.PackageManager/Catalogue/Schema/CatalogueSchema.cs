using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.PackageManager
{
    public class CatalogueSchema
    {
        public CatalogueSchema() { }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("catalogue")]
        public List<CatalogueEntrySchema> Catalogue { get; set; } = new List<CatalogueEntrySchema>();
    }
}