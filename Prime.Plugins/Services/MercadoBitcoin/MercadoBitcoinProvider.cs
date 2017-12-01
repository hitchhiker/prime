using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.MercadoBitcoin
{
    // https://www.mercadobitcoin.com.br/api-doc/
    public class MercadoBitcoinProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string MercadoBitcoinApiUrl = "https://www.mercadobitcoin.net/api/";

        private static readonly ObjectId IdHash = "prime:mercadobitcoin".GetObjectIdHashCode();
        private const string PairsCsv = "btcblr,ltcblr,bchblr";

        //Could not find a rate limiter in official documentation.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IMercadoBitcoinApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("MercadoBitcoin");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public MercadoBitcoinProvider()
        {
            ApiProvider = new RestApiClientProvider<IMercadoBitcoinApi>(MercadoBitcoinApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("BTC").ConfigureAwait(false);

            return r?.ticker != null;
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
            var r = await api.GetTickerAsync(context.Pair.Asset1.ToRemoteCode(this)).ConfigureAwait(false);

            if (r?.ticker == null)
            {
                throw new ApiResponseException("No tickers returned.", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, r.ticker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ticker.sell, r.ticker.buy, r.ticker.low, r.ticker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.ticker.vol)
            });
        }
    }
}
