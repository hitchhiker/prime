using Prime.Core;

namespace Prime.Manager.Messages
{
    public class DeleteProviderKeysRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
    }
}
