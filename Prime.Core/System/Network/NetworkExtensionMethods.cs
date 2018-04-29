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

        public static T FirstDirectProvider<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            return providers == null ? default(T) : providers.Where(x=>x.IsDirect).OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        public static T FirstProviderOf<T>(this IEnumerable<INetworkProvider> providers) where T : INetworkProvider
        {
            return providers == null ? default(T) : providers.OfType<T>().OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        public static T FirstProviderOf<T2, T>(this IEnumerable<T2> providers) where T : INetworkProvider where T2 : T
        {
            return providers == null ? default(T) : providers.OfType<T>().OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        public static IList<T> WithApi<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            var networks = UserContext.Current.ApiKeys.Select(x => x.Network);
            return providers.Where(x => networks.Any(n => x.Network.Equals(n))).ToList();
        }
    }
}