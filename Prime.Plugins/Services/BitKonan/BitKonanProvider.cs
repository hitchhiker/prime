using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitKonan
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://bitkonan.com/info/api
    public class BitKonanProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string BitKonanApiUrl = "https://bitkonan.com/api/";

        private static readonly ObjectId IdHash = "prime:bitkonan".GetObjectIdHashCode();
        private const string PairsCsv = "btcusd,ltcusd";

        private static AssetPair _assetPairBtcUsd;
        private AssetPair AssetPairBtcUsd => _assetPairBtcUsd ?? (_assetPairBtcUsd = new AssetPair("BTC", "USD", this));

        private static AssetPair _assetPairLtcUsd;
        private AssetPair AssetPairLtcUsd => _assetPairLtcUsd ?? (_assetPairLtcUsd = new AssetPair("LTC", "USD", this));

        // No information in API document.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IBitKonanApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("BitKonan");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public BitKonanProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitKonanApi>(BitKonanApiUrl, this, (k) => null);

            if(_assetPairBtcUsd == null)
                _assetPairBtcUsd = new AssetPair("BTC", "USD", this);
            if(_assetPairLtcUsd == null)
                _assetPairLtcUsd = new AssetPair("LTC", "USD", this);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("BTC_USD".ToAssetPair(this));
            var r = await GetPricingAsync(ctx).ConfigureAwait(false);

            return r != null;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        private void CheckInputAssetPair(AssetPair pair)
        {
            if (!pair.Equals(AssetPairBtcUsd) && !pair.Equals(AssetPairLtcUsd))
                throw new AssetPairNotSupportedException(pair, this);
        }

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            CheckInputAssetPair(context.Pair);

            var api = ApiProvider.GetApi(context);

             var tickerResponse = context.Pair.Equals(AssetPairBtcUsd) 
                ? await api.GetBtcTickerAsync().ConfigureAwait(false)
                : await api.GetLtcTickerAsync().ConfigureAwait(false);

            return new MarketPrices(new MarketPrice(Network, context.Pair, tickerResponse.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, tickerResponse.ask, tickerResponse.bid, tickerResponse.low, tickerResponse.high),
                Volume = new NetworkPairVolume(Network, context.Pair, tickerResponse.volume)
            });
        }

        /// <summary>
        /// Parses dictionary which items represent price and volume.
        /// </summary>
        /// <param name="raw">Raw dictionary with price and volume.</param>
        /// <param name="market">Market of order book.</param>
        /// <param name="methodName"></param>
        /// <returns>Price in quote asset and volume in base asset.</returns>
        private (decimal Price, decimal Volume) ParseOrderBookRecords(Dictionary<string, decimal> raw, AssetPair market, [CallerMemberName] string methodName = "Unknown")
        {
            var quoteCode = market.Asset2.ShortCode.ToLower();

            var prices = raw.Where(x => x.Key.Equals(quoteCode, StringComparison.OrdinalIgnoreCase)).ToList();
            if(prices.Count == 0)
                throw new ApiResponseException($"Order book response entry does not have {market.Asset2} price field",
                    this, methodName);

            var volumes = raw.Where(x => !x.Key.Equals(quoteCode, StringComparison.OrdinalIgnoreCase)).ToList();
            if (volumes.Count == 0)
                throw new ApiResponseException($"Order book response entry does not have volume field",
                    this, methodName);

            return (prices.First().Value, volumes.First().Value);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            CheckInputAssetPair(context.Pair);

            var api = ApiProvider.GetApi(context);

            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var r = await api.GetOrderBookAsync(context.Pair.Asset1.ShortCode.ToLower()).ConfigureAwait(false);

            foreach (var rBid in r.bid.Take(maxCount).Select(x => ParseOrderBookRecords(x, context.Pair)))
                orderBook.AddBid(rBid.Price, rBid.Volume, true);

            foreach (var rAsk in r.ask.Take(maxCount).Select(x => ParseOrderBookRecords(x, context.Pair)))
                orderBook.AddAsk(rAsk.Price, rAsk.Volume, true);

            return orderBook;
        }
    }
}
