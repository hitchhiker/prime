using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.StocksExchange
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://docs.google.com/document/d/1mU8ecTlzfDtT1hmZJ-dXezMudLnfD4ZeNBr_oxFwdGI/edit#
    public class StocksExchangeProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string StocksExchangeApiVersion = "api2";
        private const string StocksExchangeApiUrl = "https://stocks.exchange/" + StocksExchangeApiVersion;

        private static readonly ObjectId IdHash = "prime:stocksexchange".GetObjectIdHashCode();
        
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IStocksExchangeApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("StocksExchange");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public StocksExchangeProvider()
        {
            ApiProvider = new RestApiClientProvider<IStocksExchangeApi>(StocksExchangeApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r?.Length > 0;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentTicker in r)
            {
                pairs.Add(rCurrentTicker.market_name.ToAssetPair(this));
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No tickers returned", this);
            }

            var prices = new MarketPrices();

            var rPairsDict = r.ToDictionary(x => x.market_name.ToAssetPair(this), x => x);
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
                    prices.Add(new MarketPrice(Network, pair, currentTicker.last)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.ask, currentTicker.bid),
                        Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
                    });
                }
            }

            return prices;
        }
    }
}
