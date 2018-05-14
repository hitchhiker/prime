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
            InterfaceTypeId = ServerContext.Testing.Types.Get(interfaceType);
        }

        [Bson]
        public INetworkProvider Provider { get; set; }

        [Bson]
        public int InterfaceTypeId { get; set; }

        public override string ToString()
        {
            var i = ServerContext.Testing.Types.Get(InterfaceTypeId);
            return $"{Provider?.Title} -> {i.Name}";
        }
    }
}