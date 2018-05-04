using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.PackageManager
{
    public class PackageMetaSchema
    {
        public PackageMetaSchema() { }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}