using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Prime.Radiant
{
    public class CatalogueIndex
    {
        [JsonProperty("catalogue_type")]
        public string CatalogueType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("utc_created")]
        public DateTime UtcCreated { get; set; }

        [JsonProperty("current_revision")]
        public int CurrentRevision { get; set; }

        [JsonProperty("elems")]
        public List<CatalogueIndexEntry> Entries { get; set; } = new List<CatalogueIndexEntry>();

        public CatalogueIndexEntry GetLatest() => Entries.FirstOrDefault(x => x.Revision == CurrentRevision);
    }
}