using Prime.Core;

namespace Prime.Manager.Messages
{
    public class TestPrivateApiRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
    }
}
