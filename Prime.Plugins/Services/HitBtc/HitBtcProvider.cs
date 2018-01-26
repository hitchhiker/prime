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
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class HitBtcProvider : IPublicPricingProvider, IAssetPairsProvider
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
            ApiProvider = new RestApiClientProvider<IHitBtcApi>(HitBtcApiUrl, this, k => new HitBtcAuthenticator(k).GetRequestModifierAsync);
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

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.ToTicker(this);

            var rRaw = await api.GetTickerAsync(pairCode).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if(!r.last.HasValue)
                throw new AssetPairNotSupportedException(context.Pair, this);

            var price = new MarketPrice(Network, context.Pair, r.last.Value)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume, r.volumeQuote)
            };

            return new MarketPrices(price);
        }
                
        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAllTickersAsync().ConfigureAwait(false);

            var r = rRaw.GetContent();

            var prices = new MarketPrices();

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
        
                    prices.Add(new MarketPrice(Network, pair, ticker.last.Value)
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

                    prices.Add(new MarketPrice(Network, pair, ticker.last.Value)
                    {
                        PriceStatistics = new PriceStatistics(Network, pair.Asset2, ticker.ask, ticker.bid, ticker.low, ticker.high),
                        Volume = new NetworkPairVolume(Network, pair, ticker.volume, ticker.volumeQuote)
                    });
                }
            }

            return prices;
        }

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
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
            var rRaw = await api.GetSymbolsAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var assetPairs = new AssetPairs();

            foreach (var symbol in r)
                 assetPairs.Add(new AssetPair(symbol.baseCurrency, symbol.quoteCurrency, this));
            
            return assetPairs;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetDepositAddressAsync(context.Asset.ShortCode).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

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
            var rRaw = await api.GetBalancesAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null && r.Any();
        }
    }
}
