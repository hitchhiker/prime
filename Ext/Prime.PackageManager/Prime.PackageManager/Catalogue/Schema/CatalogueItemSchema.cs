using Newtonsoft.Json;

namespace Prime.PackageManager
{
    public class CatalogueItemSchema
    {
        public CatalogueItemSchema() { }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}