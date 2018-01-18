using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var ctx = new PublicPriceContext("btc_usd".ToAssetPair(this));
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

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            BitKonanSchema.TickerResponse tickerResponse;

            if (context.Pair.Equals(new AssetPair("BTC", "USD")))
            {
                tickerResponse = await api.GetBtcTickerAsync().ConfigureAwait(false);
            }
            else if (context.Pair.Equals(new AssetPair("LTC", "USD")))
            {
                tickerResponse = await api.GetLtcTickerAsync().ConfigureAwait(false);
            }
            else
            {
                throw new ApiResponseException("Invalid asset pair");
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, tickerResponse.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, tickerResponse.ask, tickerResponse.bid, tickerResponse.low, tickerResponse.high),
                Volume = new NetworkPairVolume(Network, context.Pair, tickerResponse.volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);
            
            if (context.Pair.Equals(new AssetPair("BTC", "USD")))
            {
                var r = await api.GetBtcOrderBookAsync().ConfigureAwait(false);
                var asks = r.ask.Take(maxCount);
                var bids = r.bid.Take(maxCount);

                foreach (var i in bids)
                    orderBook.AddBid(i.btc, 0, true);

                foreach (var i in asks)
                    orderBook.AddAsk(i.btc, 0, true);
            }
            else if (context.Pair.Equals(new AssetPair("LTC", "USD")))
            {
                var r = await api.GetLtcOrderBookAsync().ConfigureAwait(false);

                var asks = r.ask.Take(maxCount);
                var bids = r.bid.Take(maxCount);

                foreach (var i in bids)
                    orderBook.AddBid(i.ltc, 0, true);

                foreach (var i in asks)
                    orderBook.AddAsk(i.ltc, 0, true);
            }
            else
            {
                throw new ApiResponseException("Invalid asset pair");
            }

            return orderBook;
        }
    }
}
