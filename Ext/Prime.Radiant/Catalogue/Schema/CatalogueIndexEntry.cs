using System;
using Newtonsoft.Json;
using Prime.Base.DStore;

namespace Prime.Radiant
{
    public class CatalogueIndexEntry
    {
        [JsonProperty("utc_created")]
        public DateTime UtcCreated { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("curi")]
        public ContentUri Uri { get; set; }
    }
}