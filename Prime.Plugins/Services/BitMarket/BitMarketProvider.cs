using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitMarket
{
    public class BitMarketProvider : IDescribesAssets, IAssetPairsProvider
    {
        private const string BitMarketApiUrl = "https://www.bitmarket.net/";

        private RestApiClientProvider<IBitMarketApi> ApiProvider { get; }

        private const string PairsCsv = "BTC_PLN,BTC_EUR,LTC_PLN,LTC_BTC,LiteMineX_BTC";

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(PairsCsv.ToCsv().Select(x=> x.ToAssetPairRaw())));

        private static readonly ObjectId IdHash = "prime:bitmarket".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("BitMarket");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => false;
        private static readonly IAssetCodeConverter AssetCodeConverter = new BitMarketCodeConverter();

        public BitMarketProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitMarketApi>(BitMarketApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public Task<bool> TestPublicApiAsync()
        {
            return Task.Run(() => true);
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetBitMarketPairCode(context.Pair);

            try
            {
                var r = await api.GetTicker(pairCode).ConfigureAwait(false);
                return new MarketPrice(context.Pair, r.last);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    throw new NoAssetPairException(context.Pair, this);
                throw;
            }
        }

        private string GetBitMarketPairCode(AssetPair pair)
        {
            var asset1 = pair.Asset1.ToRemoteCode(this);
            var asset2 = pair.Asset2.ToRemoteCode(this);

            return $"{asset1}{asset2}";
        }
    }
}
