using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.Radiant
{
    public class PackageConfigItem
    {
        [JsonProperty("src")]
        public string Source { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "netcore";

        [JsonProperty("key", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PackageId { get; set; }

        [JsonProperty("suspend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsSuspended { get; set; }

        public ObjectId GetId()
        {
            if (!string.IsNullOrWhiteSpace(PackageId))
                return new ObjectId(PackageId);
            return !string.IsNullOrWhiteSpace(Key) ? Key.GetObjectIdHashCode(true, true) : ObjectId.Empty;
        }
    }
}