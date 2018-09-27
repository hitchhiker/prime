using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prime.Base;
using Prime.Core;

namespace Prime.Radiant
{
    public class CataloguePublisherConfig
    {
        [JsonProperty("name")]
        public string CatalogueName { get; set; }

        public ObjectId CatalogueId => $"prime-catalogue:{PubKey}".GetObjectIdHashCode(true, true);

        [JsonProperty("ipns_key_pub")]
        public string IpnsKeyPublic { get; set; }

        [JsonProperty("ipns_key_name")]
        public string IpnsKeyName { get; set; }

        [JsonProperty("public_key")]
        public string PubKey { get; set; }

        [JsonProperty("private_key")]
        public string PriKey { get; set; }
    }
}