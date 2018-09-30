using Prime.Core;

namespace Prime.Base.Messaging.Common
{
    public class PrimePublishRequest : BaseTransportRequestMessage
    {
        public string PublisherConfigPath { get; set; }
    }
}