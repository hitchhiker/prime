using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.PackageManager
{
    public class CatalogueEntrySchema
    {
        public CatalogueEntrySchema() { }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("entries")]
        public List<CatalogueItemSchema> Entries { get; set; } = new List<CatalogueItemSchema>();
    }
}