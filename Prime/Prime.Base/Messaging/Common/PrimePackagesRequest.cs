using Prime.Core;

namespace Prime.Base.Messaging.Common
{
    public class PrimePackagesRequest : BaseTransportRequestMessage
    {
        public PrimeBootOptions.Packages Options { get; set; }

        private PrimePackagesRequest() { }

        public PrimePackagesRequest(PrimeBootOptions.Packages options)
        {
            Options = options;
        }
    }
}