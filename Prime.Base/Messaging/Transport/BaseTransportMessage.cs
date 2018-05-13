using System;
using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public abstract class BaseTransportMessage
    {
        [JsonProperty("sessionId", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId SessionId { get; set; }

        [JsonIgnore]
        public bool IsRemote { get; set; }
    }
}