using Prime.Core;

namespace Prime.PackageManager.Messages
{
    public class PrimePackagesResponse : BaseTransportResponseMessage
    {
        public PrimePackagesResponse(PrimePackagesRequest request) : base(request)
        {

        }

        public bool Success { get; set; }
    }
}