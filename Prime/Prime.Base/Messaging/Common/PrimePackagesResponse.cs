using Prime.Core;

namespace Prime.Base.Messaging.Common
{
    public class PrimePackagesResponse : BaseTransportResponseMessage
    {
        public PrimePackagesResponse(PrimePackagesRequest request) : base(request)
        {

        }

        public bool Success { get; set; }
    }
}