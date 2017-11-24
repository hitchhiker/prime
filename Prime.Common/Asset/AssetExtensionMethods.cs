using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public static class AssetExtensionMethods
    {
        public static AssetPair ToAssetPair(this string pair, IDescribesAssets provider)
        {
            var delimiter = provider.CommonPairSeparator ?? '_';
            return pair.ToAssetPair(provider, delimiter);
        }

        public static AssetPair ToAssetPair(this string pair, IDescribesAssets provider, char delimiter)
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
        /// Creates asset pair from string basing on base asset length.
        /// </summary>
        /// <param name="pair">Asset pair code.</param>
        /// <param name="provider">Asset code converter.</param>
        /// <param name="firstAssetLength">The length of base asset.</param>
        /// <returns>Asset pair from specified string.</returns>
        public static AssetPair ToAssetPair(this string pair, IDescribesAssets provider, int firstAssetLength)
        {
            if (string.IsNullOrWhiteSpace(pair) || firstAssetLength < 1 || firstAssetLength > pair.Length - 1)
                return AssetPair.Empty;

            var assetCode1 = pair.Substring(0, firstAssetLength);
            var assetCode2 = pair.Replace(assetCode1, "");

            return new AssetPair(assetCode1, assetCode2, provider);
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

        /// <summary>
        /// Will search for the price in both directions (normal / reversed).
        /// </summary>
        public static MarketPrice GetPrice(this PriceGraph graph, AssetPair pair)
        {
            return graph?.Prices?.GetPrice(pair);
        }        
        
        /// <summary>
        /// Will search for the price in both directions (normal / reversed).
        /// </summary>
        public static MarketPrice GetPrice(this PriceGraph graph, Network network, AssetPair pair)
        {
            return graph?.PricesByNetwork?.Get(network)?.GetPrice(pair);
        }

        /// <summary>
        /// Will search for the price in both directions (normal / reversed).
        /// </summary>
        public static MarketPrice GetPrice(this IEnumerable<MarketPrice> prices, AssetPair pair)
        {
            var m = prices.FirstOrDefault(x => x.Pair.Id == pair.Id);
            if (m != null)
                return m;

            m = prices.FirstOrDefault(x => x.Pair.Id == pair.Reversed.Id);
            return m?.Reversed;
        }
        
        /// <summary>
        /// Will search for the price in both directions (normal / reversed).
        /// </summary>
        public static MarketPrice GetPrice(this IEnumerable<MarketPrice> prices, Network network, AssetPair pair)
        {
            var m = prices.FirstOrDefault(x => x.Pair.Id == pair.Id && x.Network.Id == network.Id);
            if (m != null)
                return m;

            m = prices.FirstOrDefault(x => x.Pair.Id == pair.Reversed.Id && x.Network.Id == network.Id);
            return m?.Reversed;
        }

        public static decimal Fx(this IEnumerable<MarketPrice> prices, AssetPair pair)
        {
            if (pair.Asset1.Id == pair.Asset2.Id)
                return 1;

            return GetPrice(prices, pair)?.Price.ToDecimalValue() ?? 0;
        }

        public static Money? FxConvert(this IEnumerable<MarketPrice> prices, Money money, Asset quote, Money? defaultValue = null)
        {
            if (money.Asset.Id == quote.Id)
                return money;

            var fx = prices.Fx(new AssetPair(money.Asset, quote));
            if (fx == 0)
                return defaultValue;

            return new Money(fx * money.ToDecimalValue(), quote);
        }
    }
}