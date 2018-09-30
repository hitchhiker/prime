using Prime.Core;

namespace Prime.Base.Messaging.Common
{
    public class PrimePublishResponse : BaseTransportResponseMessage
    {
        public PrimePublishResponse(PrimePublishRequest request) : base(request)
        {
            
        }

        public bool Success { get; set; }
    }
}