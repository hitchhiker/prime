using System;
using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public abstract class BaseTransportMessage
    {
        [JsonProperty("clientId", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId ClientId { get; set; }

        [JsonIgnore]
        public bool IsRemote { get; set; }
    }
}