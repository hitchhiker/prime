using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.HitBtc
{
    public class HitBtcProvider : IBalanceProvider, IPublicPricesProvider, IPublicPriceProvider, IPublicPriceStatistics, IAssetPairsProvider, IDepositProvider
    {
        private const string HitBtcApiUrl = "https://api.hitbtc.com/api";

        private static readonly ObjectId IdHash = "prime:hitbtc".GetObjectIdHashCode();
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private const string ErroTextrNoLatestValueForPair = "No latest value for {0} pair";

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

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();
            var r = await api.GetTicker(pairCode).ConfigureAwait(false);

            CheckNullableResult(r.last, String.Format(ErroTextrNoLatestValueForPair, context.Pair.TickerDash()));

            // TODO: implement statistics.
            return new MarketPrice(context.Pair, r.last.Value);
        }

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickers().ConfigureAwait(false);

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

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbols().ConfigureAwait(false);

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

            var r = await api.GetDepositAddress(context.Asset.ShortCode).ConfigureAwait(false);

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

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalances().ConfigureAwait(false);

            return r != null && r.balance.Any();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalances().ConfigureAwait(false);

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

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerSimple();
            var r = await api.GetTicker(pairCode).ConfigureAwait(false);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = r.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
