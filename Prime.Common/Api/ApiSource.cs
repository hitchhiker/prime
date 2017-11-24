using System;
using Prime.Utility;

namespace Prime.Common
{
    public class ApiSource
    {
        public ApiSource() { }

        public ApiSource(INetworkProvider provider, Type interfaceType)
        {
            Provider = provider;
            InterfaceTypeId = TypeCatalogue.I.Get(interfaceType);
        }

        [Bson]
        public INetworkProvider Provider { get; set; }

        [Bson]
        public int InterfaceTypeId { get; set; }

        public override string ToString()
        {
            var i = TypeCatalogue.I.Get(InterfaceTypeId);
            return $"{Provider?.Title} -> {i.Name}";
        }
    }
}