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
    public class HitBtcProvider : IBalanceProvider, IPublicPricingProvider, IAssetPairsProvider, IDepositProvider
    {
        private const string ApiVersion = "2";
        private const string HitBtcApiUrl = "https://api.hitbtc.com/api/" + ApiVersion;

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
        public char? CommonPairSeparator => null;

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
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if(!r.last.HasValue)
                throw new NoAssetPairException(context.Pair, this);

            var price = new MarketPrice(Network, context.Pair, r.last.Value)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume, r.volumeQuote)
            };

            return new MarketPricesResult(price);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllTickersAsync().ConfigureAwait(false);

            var prices = new MarketPricesResult();

            if (context.IsRequestAll)
            {
                var knownPairs = new AssetPairs();

                foreach (var ticker in r.OrderBy(x => x.symbol.Length))
                {
                    var pairCodes = AssetsUtilities.GetAssetPair(ticker.symbol, knownPairs);

                    if (!pairCodes.HasValue || !ticker.last.HasValue)
                        continue;

                    var pair = new AssetPair(pairCodes.Value.AssetCode1, pairCodes.Value.AssetCode2, this);

                    knownPairs.Add(pair);

                    prices.MarketPrices.Add(new MarketPrice(Network, pair, ticker.last.Value)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, ticker.ask, ticker.bid, ticker.low, ticker.high),
                        Volume = new NetworkPairVolume(Network, pair, ticker.volume, ticker.volumeQuote)
                    });
                }
            }
            else
            {
                foreach (var pair in context.Pairs)
                {
                    var pairCode = pair.ToTicker(this);

                    var ticker = r.FirstOrDefault(x => x.symbol.Equals(pairCode, StringComparison.OrdinalIgnoreCase));

                    if (ticker?.last == null)
                    {
                        prices.MissedPairs.Add(pair);
                        continue;
                    }

                    prices.MarketPrices.Add(new MarketPrice(Network, pair, ticker.last.Value)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, ticker.ask, ticker.bid, ticker.low, ticker.high),
                        Volume = new NetworkPairVolume(Network, pair, ticker.volume, ticker.volumeQuote)
                    });
                }
            }

            return prices;
        }

        public async Task<MarketPricesResult> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
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

            foreach (var symbol in r)
                 assetPairs.Add(new AssetPair(symbol.baseCurrency, symbol.quoteCurrency, this));
            
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

            return r != null && r.Any();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetBalancesAsync().ConfigureAwait(false);

            var balances = new BalanceResults(this);

            foreach (var rBalance in r)
            {
                balances.Add(new BalanceResult(rBalance.currency.ToAsset(this))
                {
                    Available = rBalance.available,
                    Balance = rBalance.available,
                    Reserved = rBalance.reserved
                });
            }

            return balances;
        }
    }
}
