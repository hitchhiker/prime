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

        public static T FirstProviderByVolume<T>(this IEnumerable<T> providers, AssetPair pair) where T : INetworkProvider
        {
            if (providers == null)
                return default(T);

            var apd = PublicContext.I.PubData.GetAssetPairData(pair);
            if (apd.IsMissing)
                return FirstProvider(providers);

            var voldesc = apd.Exchanges.OrderByDescending(x => x.Volume24HourTo).ToList();
            foreach (var e in voldesc)
            {
                var prov = providers.FirstOrDefault(x => Equals(x.Network, e.Network));
                if (prov != null)
                    return prov;
            }

            return FirstProvider(providers);
        }

        public static IList<T> WithApi<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            var networks = UserContext.Current.ApiKeys.Select(x => x.Network);
            return providers.Where(x => networks.Any(n => x.Network.Equals(n))).ToList();
        }
    }
}