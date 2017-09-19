using System.Collections.Generic;
using System.Linq;

namespace Prime.Core
{
    public static class NetworkExtensionMethods
    {
        public static T FirstProvider<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            return providers == null ? default(T) : providers.OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        public static IEnumerable<T> Active<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            return null;
        }
    }
}