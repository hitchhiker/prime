using System;
using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public abstract class BaseTransportMessage
    {
        [JsonProperty("clientid", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId ClientId { get; set; }

        [JsonIgnore]
        public bool IsRemote { get; set; }
    }
}