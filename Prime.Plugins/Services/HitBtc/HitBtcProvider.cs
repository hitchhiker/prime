using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.HitBtc
{
    public class HitBtcProvider : IExchangeProvider, IBalanceProvider, IOhlcProvider, IOrderBookProvider, IPublicPricesProvider
    {
        private const string HitBtcApiUrl = "https://api.hitbtc.com/api";

        private static readonly ObjectId IdHash = "prime:hitbtc".GetObjectIdHashCode();
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private const string ErroTextrNoLatestValueForPair = "No latest value for {0} pair";
        private const string ErrorTextExchangeDoesNotSupportPair = "Exchange does not support {0} pair";

        private RestApiClientProvider<IHitBtcApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("HitBTC");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName { get; } = null;
        public string Title => Network.Name;
        public IRateLimiter RateLimiter => Limiter;
        
        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => false;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;
        public bool IsDirect => true;

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();
            var r = await api.GetTicker(pairCode);

            CheckNullableResult(r.last, String.Format(ErroTextrNoLatestValueForPair, context.Pair.TickerDash()));

            return new MarketPrice(context.Pair, r.last.Value);
        }

        public async Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickers();

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var pairCode = pair.TickerSimple();

                var tickers = r.Where(x => x.Key.Equals(pairCode)).ToArray();

                if (!tickers.Any())
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var ticker = tickers.First();

                CheckNullableResult(ticker.Value.last, String.Format(ErroTextrNoLatestValueForPair, pair.TickerDash()));

                prices.MarketPrices.Add(new MarketPrice(pair, ticker.Value.last.Value));
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

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbols();

            var assetPairs = new AssetPairs();

            foreach (var symbol in r.symbols)
            {
                assetPairs.Add(new AssetPair(symbol.commodity.ToAsset(this), symbol.currency.ToAsset(this)));
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

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();
            var r = await api.GetTicker(pairCode);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
