using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Plugins.Services.Base;
using Prime.Utility;

namespace Prime.Plugins.Services.Liqui
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://liqui.io/api
    public partial class LiquiProvider : BaseProvider<ILiquiApi>
    {
        private const string LiquiApiVersion = "3";
        private const string LiquiApiUrl = "https://api.liqui.io/api/" + LiquiApiVersion;
        private const string LiquiApiUrlPrivate = "https://api.liqui.io/tapi";

        private static readonly ObjectId IdHash = "prime:liqui".GetObjectIdHashCode();

        public override Network Network { get; } = Networks.I.Get("Liqui");

        public override ObjectId Id => IdHash;
        protected override RestApiClientProvider<ILiquiApi> ApiProviderPublic { get; }
        protected override RestApiClientProvider<ILiquiApi> ApiProviderPrivate { get; }

        public LiquiProvider() : base()
        {
            ApiProviderPrivate = new RestApiClientProvider<ILiquiApi>(LiquiApiUrlPrivate, this, (k) => new LiquiAuthenticator(k).GetRequestModifierAsync);
            ApiProviderPublic = new RestApiClientProvider<ILiquiApi>(LiquiApiUrl, this, (k) => null);
        }

        //public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        //{
        //    var api = ApiProvider.GetApi(context);
        //    var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

        //    return r?.pairs?.Count > 0;
        //}

        //public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        //{
        //    var api = ApiProvider.GetApi(context);

        //    var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

        //    if (r?.pairs == null || r.pairs.Count == 0)
        //    {
        //        throw new ApiResponseException("No asset pairs returned", this);
        //    }

        //    var pairs = new AssetPairs();

        //    foreach (var rCurrentTicker in r.pairs)
        //    {
        //        pairs.Add(rCurrentTicker.Key.ToAssetPair(this));
        //    }

        //    return pairs;
        //}

        //public IAssetCodeConverter GetAssetCodeConverter()
        //{
        //    return null;
        //}

        public override PricingFeatures PricingFeatures { get; } = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
        };

        public override async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            return await GetPriceAsync(context).ConfigureAwait(false);
        }

        //public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        //{
        //    var api = ApiProvider.GetApi(context);
        //    var pairCode = context.Pair.ToTicker(this).ToLower();
        //    var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
            
        //    if(!r.TryGetValue(pairCode, out var rTicker))
        //        throw new AssetPairNotSupportedException(context.Pair, this);
            
        //    return new MarketPrices(new MarketPrice(Network, context.Pair, rTicker.last)
        //    {
        //        PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, rTicker.sell, rTicker.buy, rTicker.low, rTicker.high),
        //        Volume = new NetworkPairVolume(Network, context.Pair, rTicker.vol)
        //    });
        //}

        //public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        //{
        //    var api = ApiProvider.GetApi(context);
        //    var pairsCsv = string.Join("-", context.Pairs.Select(x => x.ToTicker(this).ToLower()));
        //    var r = await api.GetTickersAsync(pairsCsv).ConfigureAwait(false);

        //    if (r == null || r.Count == 0)
        //    {
        //        throw new ApiResponseException("No tickers returned", this);
        //    }

        //    var prices = new MarketPrices();

        //    foreach (var pair in context.Pairs)
        //    {
        //        var currentTicker = r.FirstOrDefault(x => x.Key.ToAssetPair(this).Equals(pair)).Value;

        //        if (currentTicker == null)
        //        {
        //            prices.MissedPairs.Add(pair);
        //        }
        //        else
        //        {
        //            prices.Add(new MarketPrice(Network, pair, currentTicker.last)
        //            {
        //                PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.sell, currentTicker.buy, currentTicker.low, currentTicker.high),
        //                Volume = new NetworkPairVolume(Network, pair, currentTicker.vol)
        //            });
        //        }
        //    }

        //    return prices;
        //}

        //public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        //{
        //    var api = ApiProvider.GetApi(context);
        //    var pairCode = context.Pair.ToTicker(this).ToLower();

        //    var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
        //    var orderBook = new OrderBook(Network, context.Pair);

        //    var maxCount = Math.Min(1000, context.MaxRecordsCount);

        //    r.TryGetValue(pairCode, out var response);

        //    var asks = response.asks.Take(maxCount);
        //    var bids = response.bids.Take(maxCount);

        //    foreach (var i in bids.Select(GetBidAskData))
        //        orderBook.AddBid(i.Item1, i.Item2, true);

        //    foreach (var i in asks.Select(GetBidAskData))
        //        orderBook.AddAsk(i.Item1, i.Item2, true);

        //    return orderBook;
        //}

        //private Tuple<decimal, decimal> GetBidAskData(decimal[] data)
        //{
        //    decimal price = data[0];
        //    decimal amount = data[1];

        //    return new Tuple<decimal, decimal>(price, amount);
        //}
    }
}
