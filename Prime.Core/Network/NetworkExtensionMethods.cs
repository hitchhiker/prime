using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Core
{
    public static class NetworkExtensionMethods
    {
        [Obsolete("Testing")]
        public static IList<T> WithApi<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            var networks = UserContext.Testing.ApiKeys.Select(x => x.Network);
            return providers.Where(x => networks.Any(n => x.Network.Equals(n))).ToList();
        }
    }
}