using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public static class AssetExtensionMethods
    {
        public static AssetPair ToAssetPair(this string pair, IDescribesAssets provider, char delimiter = '_')
        {
            if (string.IsNullOrWhiteSpace(pair) || pair.IndexOf(delimiter) == -1)
                return AssetPair.Empty;

            var p = pair.Split(delimiter);
            return p.Length != 2 ? AssetPair.Empty : new AssetPair(p[0], p[1], provider);
        }

        public static AssetPair ToAssetPairRaw(this string pair, char delimiter = '_')
        {
            var p = pair.Split(delimiter);
            return p.Length != 2 ? AssetPair.Empty : new AssetPair(p[0], p[1]);
        }

        /// <summary>
        /// Returns the assetCode as an Asset object, no remote provider conversion of codes is done. 
        /// Use with caution.
        /// </summary>
        /// <param name="assetCode"></param>
        /// <returns></returns>
        public static Asset ToAssetRaw(this string assetCode)
        {
            return Assets.I.GetRaw(assetCode);
        }

        public static IReadOnlyList<Asset> ToAssetsCsvRaw(this string assetCsv)
        {
            return assetCsv.ToCsv(true).Select(x => x.ToAssetRaw()).ToList();
        }

        public static IReadOnlyList<AssetPair> ToAssetPairsCsvRaw(this string assetCsv, char delimiterPair = '_')
        {
            return assetCsv.ToCsv(true).Select(x => x.ToAssetPairRaw(delimiterPair)).ToList();
        }

        public static Asset ToAsset(this string assetCode, IDescribesAssets provider)
        {
            if (provider == null)
                throw new ArgumentException(nameof(provider));

            var cv = provider.GetAssetCodeConverter();
            if (cv == null)
                return Assets.I.Get(assetCode, provider);

            return Assets.I.Get(cv.ToLocalCode(assetCode), provider);
        }

        public static string ToRemoteCode(this Asset asset, IDescribesAssets provider)
        {
            if (provider == null)
                throw new ArgumentException(nameof(provider));

            var cv = provider.GetAssetCodeConverter();
            if (cv == null)
                return asset.ShortCode;

            return ToRemoteCode(asset, cv);
        }

        public static string ToRemoteCode(this Asset asset, IAssetCodeConverter converter)
        {
            if (converter == null)
                throw new ArgumentException(nameof(converter));

            return converter.ToRemoteCode(asset);
        }

        public static bool IsNone(this Asset asset)
        {
            return asset == null || Equals(asset, Asset.None);
        }

        public static Asset EqualOrPegged(this IEnumerable<Asset> items, Asset asset)
        {
            foreach (var i in items)
            {
                if (i.Equals(asset))
                    return i;
                if (asset.Pegged.Contains(i))
                    return i;
            }
            return null;
        }

        public static decimal Fx(this IEnumerable<MarketPrice> prices, AssetPair pair)
        {
            if (pair.Asset1.Id == pair.Asset2.Id)
                return 1;

            var m = prices.FirstOrDefault(x => x.Pair.Id == pair.Id)?.Price;
            if (m != null)
                return m.Value.ToDecimalValue();

            m = prices.FirstOrDefault(x => x.Pair.Id == pair.Reversed.Id)?.Price;
            if (m != null)
                return m.Value.ReverseAsset(pair.Asset2).ToDecimalValue();

            return 0;
        }

        public static Money FxConvert(this IEnumerable<MarketPrice> prices, Money money, Asset quote)
        {
            if (money.Asset.Id == quote.Id)
                return money;

            var fx = prices.Fx(new AssetPair(money.Asset, quote));
            if (fx == 0)
                return Money.Zero;

            return new Money(fx * money.ToDecimalValue(), quote);
        }
    }
}