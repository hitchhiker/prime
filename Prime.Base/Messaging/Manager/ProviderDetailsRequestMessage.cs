using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderDetailsRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
    }
}
