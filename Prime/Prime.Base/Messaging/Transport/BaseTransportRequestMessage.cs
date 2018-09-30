using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public abstract class BaseTransportRequestMessage : BaseTransportMessage
    {
        protected BaseTransportRequestMessage()
        {
            RequestId = ObjectId.NewObjectId();
        }

        [JsonProperty("requestId", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId RequestId { get; set; }
    }
}