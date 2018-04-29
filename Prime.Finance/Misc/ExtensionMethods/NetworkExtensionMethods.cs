using System.Collections.Generic;
using System.Linq;
using Prime.Core;

namespace Prime.Finance
{
    public static class NetworkExtensionMethods
    {
        public static List<T> OrderByVolume<T>(this IEnumerable<T> providers, AssetPair pair) where T : INetworkProvider
        {
            var r = new List<T>();

            if (providers == null)
                return r;

            var apd = FinanceCommon.I.PubData.GetAggAssetPairData(pair);
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

            var apd = FinanceCommon.I.PubData.GetAggAssetPairData(pair);
            if (apd.IsMissing)
                return providers.FirstProviderOf<T, T>();

            var voldesc = apd.Exchanges.OrderByDescending(x => x.Volume24HourTo).ToList();
            foreach (var e in voldesc)
            {
                var prov = providers.FirstOrDefault(x => Equals(x.Network, e.Network));
                if (prov != null)
                    return prov;
            }

            return providers.FirstProviderOf<T, T>();
        }
    }
}