using System;
using Prime.Core;

namespace Prime.Core
{
    public class ApiSource
    {
        public ApiSource() { }

        public ApiSource(INetworkProvider provider, Type interfaceType)
        {
            Provider = provider;
            InterfaceTypeId = PrimeContext.Testing.Types.Get(interfaceType);
        }

        [Bson]
        public INetworkProvider Provider { get; set; }

        [Bson]
        public string InterfaceTypeId { get; set; }

        public override string ToString()
        {
            var i = PrimeContext.Testing.Types.Get(InterfaceTypeId);
            return $"{Provider?.Title} -> {i.Name}";
        }
    }
}