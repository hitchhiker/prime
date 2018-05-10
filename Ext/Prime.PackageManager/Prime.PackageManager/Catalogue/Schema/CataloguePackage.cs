using System.Collections.Generic;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.PackageManager
{
    public class CataloguePackage
    {
        public CataloguePackage() { }

        [JsonProperty("id"), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("instances")]
        public List<CatalogueInstance> Instances { get; set; } = new List<CatalogueInstance>();
    }
}