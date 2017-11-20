using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.HitBtc
{
    // https://api.hitbtc.com/
    public class HitBtcProvider : IBalanceProvider, IPublicPricingProvider, IAssetPairsProvider, IDepositProvider, IAssetPairVolumeProvider
    {
        private const string HitBtcApiUrl = "https://api.hitbtc.com/api";

        private static readonly ObjectId IdHash = "prime:hitbtc".GetObjectIdHashCode();
        private static readonly IRateLimiter Limiter = new NoRateLimits();

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

        public HitBtcProvider()
        {
            ApiProvider = new RestApiClientProvider<IHitBtcApi>(HitBtcApiUrl, this, k => new HitBtcAuthenticator(k).GetRequestModifier);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this, "");
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r.last.HasValue == false)
                throw new NoAssetPairException(context.Pair, this);

            var price = new MarketPrice(Network, context.Pair, r.last.Value)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume, r.volume_quote)
            };

            return new MarketPricesResult(price);
        }

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickersAsync().ConfigureAwait(false);

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var pairCode = pair.ToTicker(this, "");

                var tickers = r.Where(x => x.Key.Equals(pairCode, StringComparison.OrdinalIgnoreCase)).ToArray();

                if (!tickers.Any())
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var ticker = tickers.FirstOrDefault();

                if (ticker.Value?.last == null)
                    continue;

                prices.MarketPrices.Add(new MarketPrice(Network, pair, ticker.Value.last.Value)
                {
                    PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, ticker.Value.ask, ticker.Value.bid, ticker.Value.low, ticker.Value.high),
                    Volume = new NetworkPairVolume(Network, context.Pair, ticker.Value.volume, ticker.Value.volume_quote)
                });
            }

            return prices;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbolsAsync().ConfigureAwait(false);

            var assetPairs = new AssetPairs();

            foreach (var symbol in r.symbols)
                 assetPairs.Add(new AssetPair(symbol.commodity.ToAsset(this), symbol.currency.ToAsset(this)));
            
            return assetPairs;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetDepositAddressAsync(context.Asset.ShortCode).ConfigureAwait(false);

            var walletAddresses = new WalletAddresses
            {
                new WalletAddress(this, context.Asset)
                {
                    Address = r.address
                }
            };

            return walletAddresses;
        }

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context)
        {
            return Task.FromResult<TransferSuspensions>(null);
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalancesAsync().ConfigureAwait(false);

            return r != null && r.balance.Any();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalancesAsync().ConfigureAwait(false);

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

        public async Task<NetworkPairVolume> GetAssetPairVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this, "");
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            return new NetworkPairVolume(Network, context.Pair, r.volume, r.volume_quote);
        }
    }
}
