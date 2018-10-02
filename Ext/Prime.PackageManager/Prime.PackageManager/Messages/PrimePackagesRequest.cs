using Prime.Core;

namespace Prime.PackageManager.Messages
{
    public class PrimePackagesRequest : BaseTransportRequestMessage
    {
        public PackageManagerArguments Arguments { get; set; }

        private PrimePackagesRequest() { }

        public PrimePackagesRequest(PackageManagerArguments arguments)
        {
            Arguments = arguments;
        }
    }
}