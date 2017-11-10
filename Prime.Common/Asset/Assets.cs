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

        public Asset None;

        private Asset _btc;
        public Asset Btc => _btc ?? (_btc = GetRaw("BTC"));

        private Asset _eth;
        public Asset Eth => _eth ?? (_eth = GetRaw("Eth"));

        private Asset _xrp;
        public Asset Xrp => _xrp ?? (_xrp = GetRaw("Xrp"));

        private Asset _eur;
        public Asset Eur => _eur ?? (_eur = GetRaw("Eur"));

        private Asset _jpy;
        public Asset Jpy => _jpy ?? (_jpy = GetRaw("Jpy"));

        private Asset _usd;
        public Asset Usd => _usd ?? (_usd = GetRaw("Usd"));

        private Asset _usdT;
        public Asset UsdT => _usdT ?? (_usdT = GetRaw("UsdT"));

        private Asset _krw;
        public Asset Krw => _krw ?? (_krw = GetRaw("Krw"));
    }
}