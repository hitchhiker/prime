using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Bitfinex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://bitfinex.readme.io/v1/reference
    public partial class BitfinexProvider : IPublicPricingProvider, IAssetPairsProvider, IOrderBookProvider
    {
        private const string BitfinexApiVersion = "v1";
        private const string BitfinexApiUrl = "https://api.bitfinex.com/" + BitfinexApiVersion;

        private static readonly ObjectId IdHash = "prime:bitfinex".GetObjectIdHashCode();

        //If an IP address exceeds a certain number of requests per minute (between 10 and 90) to a specific REST API endpoint e.g., /ticker, the requesting IP address will be blocked for 10-60 seconds on that endpoint and the JSON response {"error": "ERR_RATE_LIMIT"} will be returned. 
        //Please note the exact logic and handling for such DDoS defenses may change over time to further improve reliability.
        //https://bitfinex.readme.io/v1/docs
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(90, 1);

        private RestApiClientProvider<IBitfinexApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bitfinex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BitfinexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitfinexApi>(BitfinexApiUrl, this, k => new BitfinexAuthenticator(k).GetRequestModifierAsync);
        }

        private void CheckBitfinexResponseErrors<T>(Response<T> rawResponse, [CallerMemberName]string methodName = null)
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                if (rawResponse.GetContent() is BitfinexSchema.BaseResponse rBase)
                {
                    var message = String.IsNullOrWhiteSpace(rBase.message) ? "Unknown error occurred" : rBase.message;
                    throw new ApiResponseException(message, this, methodName);
                }
                else
                {
                    var reason = rawResponse.ResponseMessage.ReasonPhrase;
                    throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(String.IsNullOrWhiteSpace(reason) ? "": $" ({reason})")}", this, methodName);
                }
            }               
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAssetsAsync().ConfigureAwait(false);

            CheckBitfinexResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r?.Length > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);

            var infoRequest = new BitfinexSchema.AccountInfoRequest.Descriptor();

            var rRaw = await api.GetAccountInfoAsync(infoRequest).ConfigureAwait(false);

            CheckBitfinexResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.Any() && r.First().fees.Any();
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetAssetsAsync().ConfigureAwait(false);

            CheckBitfinexResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (r == null || r.Length == 0)
            {
                throw new ApiResponseException("No asset pairs returned", this);
            }

            var pairs = new AssetPairs();

            foreach (var rCurrentAssetPair in r)
            {
                pairs.Add(rCurrentAssetPair.ToAssetPair(this, 3));
            }

            return pairs;
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
            var pairCode = context.Pair.ToTicker(this);
            var rRaw = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            CheckBitfinexResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.last_price)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            });
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = Math.Min(1000, context.MaxRecordsCount);

            var asks = r.asks.Take(maxCount);
            var bids = r.bids.Take(maxCount);

            foreach (var i in bids)
                orderBook.AddBid(i.price, i.amount, true);

            foreach (var i in asks)
                orderBook.AddAsk(i.price, i.amount, true);

            return orderBook;
        }
    }
}
