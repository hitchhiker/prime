using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.HitBtc
{
    public class HitBtcProvider : IExchangeProvider, IWalletService, IOhlcProvider, IOrderBookProvider, IPublicPairsPricesProvider
    {
        private const string HitBtcApiUrl = "https://api.hitbtc.com/api";

        private static readonly ObjectId IdHash = "prime:hitbtc".GetObjectIdHashCode();
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private const string ErroTextrNoLatestValueForPair = "No latest value for {0} pair";
        private const string ErrorTextExchangeDoesNotSupportPair = "Exchange does not support {0} pair";

        private RestApiClientProvider<IHitBtcApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("HitBTC");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName { get; } = null;
        public string Title => Network.Name;
        public IRateLimiter RateLimiter => Limiter;

        public bool CanMultiDepositAddress => false;
        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => false;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<LatestPrice> GetPairPriceAsync(PublicPairPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();
            var r = await api.GetTicker(pairCode);

            CheckNullableResult(r.last, String.Format(ErroTextrNoLatestValueForPair, context.Pair.TickerDash()));

            var latestPrice = new LatestPrice()
            {
                UtcCreated = DateTime.UtcNow,
                BaseAsset = context.Pair.Asset1,
                Price = new Money(r.last.Value, context.Pair.Asset2)
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickers();

            var prices = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var pair = context.QuoteAsset.ToPair(asset);
                var pairCode = pair.TickerSimple();

                var tickers = r.Where(x => x.Key.Equals(pairCode)).ToArray();

                if (!tickers.Any())
                    throw new ApiResponseException(String.Format(ErrorTextExchangeDoesNotSupportPair, pair.TickerDash()), this);

                var ticker = tickers.First();

                CheckNullableResult(ticker.Value.last, String.Format(ErroTextrNoLatestValueForPair, pair.TickerDash()));

                prices.Add(new Money(ticker.Value.last.Value, asset));
            }

            var latestPrices = new LatestPrices()
            {
                UtcCreated = DateTime.UtcNow,
                BaseAsset = context.QuoteAsset,
                Prices = prices
            };

            return latestPrices;
        }

        public async Task<List<LatestPrice>> GetPairsPricesAsync(PublicPairsPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickers();

            var prices = new List<LatestPrice>();

            foreach (var pair in context.Pairs)
            {
                var pairCode = pair.TickerSimple();

                var tickers = r.Where(x => x.Key.Equals(pairCode)).ToArray();

                if (!tickers.Any())
                    throw new ApiResponseException(String.Format(ErrorTextExchangeDoesNotSupportPair, pair.TickerDash()), this);

                var ticker = tickers.First();

                CheckNullableResult(ticker.Value.last, String.Format(ErroTextrNoLatestValueForPair, pair.TickerDash()));

                prices.Add(new LatestPrice()
                {
                    UtcCreated = DateTime.UtcNow,
                    BaseAsset = pair.Asset1,
                    Price = new Money(ticker.Value.last.Value, pair.Asset2)
                });
            }

            return prices;
        }

        private void CheckNullableResult<T>(T? value, string message) where T : struct
        {
            if (value.HasValue == false)
                throw new ApiResponseException(message, this);
        }

        public HitBtcProvider()
        {
            ApiProvider = new RestApiClientProvider<IHitBtcApi>(HitBtcApiUrl, this, k => new HitBtcAuthenticator(k).GetRequestModifier);
        }

        public Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbols();

            var assetPairs = new AssetPairs();

            foreach (var symbol in r.symbols)
            {
                assetPairs.Add(new AssetPair(symbol.currency.ToAsset(this), symbol.commodity.ToAsset(this)));
            }

            return assetPairs;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetDepositAddress(context.Asset.ShortCode);

            var walletAddresses = new WalletAddresses();
            walletAddresses.Add(new WalletAddress(this, context.Asset)
            {
                Address = r.address
            });

            return walletAddresses;
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalances();

            return r != null && r.balance.Any();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalances();

            var balances = new BalanceResults(this);

            foreach (var rBalance in r.balance)
            {
                balances.Add(new BalanceResult(rBalance.currency_code.ToAsset(this))
                {
                    Available = rBalance.balance,
                    Balance = rBalance.balance,
                    Reserved = 0
                });
            }

            return balances;
        }

        public Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            throw new NotImplementedException();
        }
    }
}
