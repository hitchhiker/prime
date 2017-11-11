using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Cex
{
    public class CexProvider : IPublicPricesProvider, IAssetPairsProvider, IPublicPriceProvider, IAssetPairVolumeProvider
    {
        private const string CexApiUrl = "https://cex.io/api";

        private static readonly ObjectId IdHash = "prime:cex".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("CEX.IO");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        private RestApiClientProvider<ICexApi> ApiProvider { get; }

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public CexProvider()
        {
            ApiProvider = new RestApiClientProvider<ICexApi>(CexApiUrl, this, k => null);
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickers().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rTicker in r.data)
            {
                pairs.Add(rTicker.pair.ToAssetPair(this, ':'));
            }

            return pairs;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = GetCexTicker(context.Pair);

            var r = await api.GetLastPrice(pairCode).ConfigureAwait(false);

            return new MarketPrice(Network, context.Pair, r.lprice);
        }

        private string GetCexTicker(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this)}/{pair.Asset2.ToRemoteCode(this)}";
        }

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetLastPrices().ConfigureAwait(false);
            CheckResponseError(r);

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var rPair = r.data.FirstOrDefault(x =>
                    x.symbol1.ToAsset(this).Equals(pair.Asset1) && 
                    x.symbol2.ToAsset(this).Equals(pair.Asset2));

                if (rPair == null)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.MarketPrices.Add(new MarketPrice(Network, pair, rPair.lprice));
            }

            return prices;
        }

        private void CheckResponseError(CexSchema.BaseResponse response)
        {
            if(!response.ok.Equals("ok")) 
                throw new ApiResponseException($"Error occurred in provider: {response.ok}", this);
        }

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickers().ConfigureAwait(false);

            CheckResponseError(r);

            var rTicker = r.data.FirstOrDefault(x => x.pair.ToAssetPair(this, ':').Equals(context.Pair));

            if(rTicker == null)
                throw new NoAssetPairException(context.Pair, this);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = rTicker.volume,
                Period = VolumePeriod.Day
            };
        }
    }
}
