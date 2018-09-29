using Prime.Core;

namespace Prime.Base.Messaging.Common
{
    public class PrimeUpdateResponse : BaseTransportResponseMessage
    {
        public PrimeUpdateResponse(PrimeUpdateRequest request) : base(request)
        {

        }

        public bool Success { get; set; }
    }
}