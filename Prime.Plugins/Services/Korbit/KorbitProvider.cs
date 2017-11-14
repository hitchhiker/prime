using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Plugins.Services.Base;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Korbit
{
    public class KorbitProvider : IOrderBookProvider, IPublicPriceProvider, IAssetPairsProvider, IPublicPriceStatistics
    {
        private static readonly ObjectId IdHash = "prime:korbit".GetObjectIdHashCode();
        private static readonly string _pairs = "btckrw,etckrw,ethkrw,xrpkrw";
        
        public const string KorbitApiVersion = "v1/";
        public const string KorbitApiUrl = "https://api.korbit.co.kr/" + KorbitApiVersion;

        private RestApiClientProvider<IKorbitApi> ApiProvider { get; }

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Korbit");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        // https://apidocs.korbit.co.kr/#first_section
        // ... Ticker calls are limited to 60 calls per 60 seconds. ...
        public IRateLimiter RateLimiter { get; } = new PerMinuteRateLimiter(60, 1);
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        public KorbitProvider()
        {
            ApiProvider = new RestApiClientProvider<IKorbitApi>(KorbitApiUrl, this, k => new KorbitAuthenticator(k).GetRequestModifier);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetKorbitTicker(context.Pair);

            try
            {
                var r = await api.GetDetailedTickerAsync(pairCode).ConfigureAwait(false);

                var sTimeStamp = r.timestamp / 1000; // r.timestamp is returned in ms.

                return new MarketPrice(Network, context.Pair, r.last, sTimeStamp.ToUtcDateTime())
                {
                    PriceStatistics = new PriceStatistics(context.QuoteAsset, r.volume, null, r.ask, r.bid, r.low, r.high)
                };
            }
            catch (ApiException ex)
            {
                if(ex.StatusCode == HttpStatusCode.BadRequest)
                    throw new NoAssetPairException(context.Pair, this);
                throw;
            }
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetKorbitTicker(context.Pair);

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);

            var bids = context.MaxRecordsCount.HasValue 
                ? r.bids.Take(context.MaxRecordsCount.Value / 2) 
                : r.bids;

            var asks = context.MaxRecordsCount.HasValue
                ? r.asks.Take(context.MaxRecordsCount.Value / 2)
                : r.asks;

            var orderBook = new OrderBook();

            var dateTime = (r.timestamp / 1000).ToUtcDateTime();

            foreach (var rBid in bids)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Price = new Money(rBid[0], context.Pair.Asset2),
                        Time = dateTime,
                        Volume = rBid[1]
                    }
                });
            }

            foreach (var rAsk in asks)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Price = new Money(rAsk[0], context.Pair.Asset2),
                        Time = dateTime,
                        Volume = rAsk[1]
                    }
                });
            }

            return orderBook;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private string GetKorbitTicker(AssetPair pair)
        {
            return new AssetPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this)).TickerUnderslash().ToLower();
        }
    }
}
