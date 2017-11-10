using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Utility;
using System.Linq;

namespace Prime.Common
{
    public class Assets
    {
        private Assets()
        {
            None = Asset.InstanceRaw("###");
            _cache.Add(None.ShortCode, None);
            Btc = GetRaw("BTC");
            Eth = GetRaw("ETH");
            Xrp = GetRaw("XRP");
            Eur = GetRaw("EUR");
            Jpy = GetRaw("JPY");
            Usd = GetRaw("USD");
            UsdT = GetRaw("USDT");
            Krw = GetRaw("KRW");
        }

        public static Assets I => Lazy.Value;
        private static readonly Lazy<Assets> Lazy = new Lazy<Assets>(()=>new Assets());

        private readonly ConcurrentDictionary<string, Asset> _cache = new ConcurrentDictionary<string, Asset>();

        private IReadOnlyList<Asset> _popular;
        public IReadOnlyList<Asset> Popular => _popular ?? (_popular = "BTC,ETH,XRP,LTC,USD,EUR,JPY,USDT".ToAssetsCsvRaw());

        /// <summary>
        /// Returns the assetCode as an Asset object, no remote provider conversion of codes is done. 
        /// Use with caution.
        /// </summary>
        /// <param name="assetCode"></param>
        /// <returns></returns>
        public Asset GetRaw(string assetCode)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
                return None;

            assetCode = assetCode.ToUpper();
            var contains = _cache.ContainsKey(assetCode);
            var a = _cache.GetOrAdd(assetCode, Asset.InstanceRaw);

            if (!contains)
                DefaultMessenger.I.Default.Send(new AssetFoundMessage(a));
            
            return a;
        }

        public Asset Get(string assetCode, IDescribesAssets provider)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
                return Asset.None;

            if (provider == null)
                throw new ArgumentException(nameof(provider));

            var cv = provider.GetAssetCodeConverter();
            if (cv == null)
                return GetRaw(assetCode);

            assetCode = cv.ToLocalCode(assetCode);

            return _cache.GetOrAdd(assetCode, Asset.InstanceRaw);
        }

        public IReadOnlyList<Asset> Cached()
        {
            return _cache.Values.ToUniqueList();
        }

        public readonly Asset None;

        public readonly Asset Btc;

        public readonly Asset Eth; 

        public readonly Asset Xrp;

        public readonly Asset Eur;

        public readonly Asset Jpy;

        public readonly Asset Usd;

        public readonly Asset UsdT;

        public readonly Asset Krw ;
    }
}