using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Xbtce
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://www.xbtce.com/tradeapi
    public class XbtceProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string XbtceApiVersion = "v1";
        private const string XbtceApiUrl = "https://cryptottlivewebapi.xbtce.net:8443/api/" + XbtceApiVersion;

        private static readonly ObjectId IdHash = "prime:xbtce".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private IXbtceApi _apiProvider;
        private IXbtceApi ApiProvider => _apiProvider ?? (_apiProvider = RestClient.For<IXbtceApi>(new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        {
            BaseAddress = new Uri(XbtceApiUrl)
        }));

        public Network Network { get; } = Networks.I.Get("Xbtce");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;
        
        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {

            var r = await ApiProvider.GetTickersAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var r = await ApiProvider.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                pairs.Add(rCurrentTicker.Symbol.ToAssetPair(this, 3));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var pairsCsv = string.Join(" ", context.Pairs.Select(x => x.ToTicker(this)));
            var r = await ApiProvider.GetTickerAsync(pairsCsv).ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r[0].LastBuyPrice)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r[0].BestAsk, r[0].BestBid),
                Volume = new NetworkPairVolume(Network, context.Pair, r[0].DailyTradedTotalVolume)
            });
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var r = await ApiProvider.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            var rPairsDict = r.ToDictionary(x => x.Symbol.ToAssetPair(this, 3), x => x);
            var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

            foreach (var pair in pairsQueryable)
            {
                rPairsDict.TryGetValue(pair, out var currentTicker);

                if (currentTicker == null)
                {
                    prices.MissedPairs.Add(pair);
                }
                else
                {
                    prices.Add(new MarketPrice(Network, pair, currentTicker.LastBuyPrice)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, r[0].BestAsk, r[0].BestBid),
                        Volume = new NetworkPairVolume(Network, pair, r[0].DailyTradedTotalVolume)
                    });
                }
            }

            return prices;
        }
    }
}
