using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public abstract class BaseTransportResponseMessage : BaseTransportMessage
    {
        protected BaseTransportResponseMessage() { }

        protected BaseTransportResponseMessage(BaseTransportRequestMessage request)
        {
            SessionId = request.SessionId;
            RequestId = request.RequestId;
        }

        [JsonProperty("requestId", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId RequestId { get; set; }
    }
}