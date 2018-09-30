using System.Collections.Generic;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.PackageManager
{
    public class PackageEntry
    {
        public PackageEntry() { }

        [JsonProperty("id"), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("instances")]
        public List<PackageInstance> PackageInstances { get; set; } = new List<PackageInstance>();
    }
}