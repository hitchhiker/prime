using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Core
{
    public class Assets
    {
        private Assets() {}

        public static Assets I => Lazy.Value;
        private static readonly Lazy<Assets> Lazy = new Lazy<Assets>(()=>new Assets());

        private readonly ConcurrentDictionary<string, Asset> _cache = new ConcurrentDictionary<string, Asset>();
        
        /// <summary>
        /// Returns the assetCode as an Asset object, no remote provider conversion of codes is done. 
        /// Use with caution.
        /// </summary>
        /// <param name="assetCode"></param>
        /// <returns></returns>
        public Asset GetRaw(string assetCode)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
                return Asset.None;

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
    }
}