using System.Collections.Generic;
using System.Linq;

namespace Prime.Common
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

        public static List<T> OrderByVolume<T>(this IEnumerable<T> providers, AssetPair pair) where T : INetworkProvider
        {
            var r = new List<T>();

            if (providers == null)
                return r;

            var apd = PublicContext.I.PubData.GetAggAssetPairData(pair);
            if (apd.IsMissing)
                return providers.ToList();

            var voldesc = apd.Exchanges.OrderByDescending(x => x.Volume24HourTo).ToList();
            foreach (var e in voldesc)
            {
                var prov = providers.FirstOrDefault(x => Equals(x.Network, e.Network));
                if (prov != null)
                    r.Add(prov);
            }

            r.AddRange(r.Except(providers));

            return r;
        }

        public static T FirstProviderByVolume<T>(this IEnumerable<T> providers, AssetPair pair) where T : INetworkProvider
        {
            if (providers == null)
                return default(T);

            var apd = PublicContext.I.PubData.GetAggAssetPairData(pair);
            if (apd.IsMissing)
                return FirstProviderOf<T, T>(providers);

            var voldesc = apd.Exchanges.OrderByDescending(x => x.Volume24HourTo).ToList();
            foreach (var e in voldesc)
            {
                var prov = providers.FirstOrDefault(x => Equals(x.Network, e.Network));
                if (prov != null)
                    return prov;
            }

            return FirstProviderOf<T, T>(providers);
        }

        public static IList<T> WithApi<T>(this IEnumerable<T> providers) where T : INetworkProvider
        {
            var networks = UserContext.Current.ApiKeys.Select(x => x.Network);
            return providers.Where(x => networks.Any(n => x.Network.Equals(n))).ToList();
        }
    }
}