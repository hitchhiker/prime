using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderSaveKeysRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
        
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
    }
}