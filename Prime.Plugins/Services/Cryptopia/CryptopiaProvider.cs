using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Cryptopia
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://www.cryptopia.co.nz/Forum/Thread/255
    public partial class CryptopiaProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string CryptopiaApiUrl = "https://www.cryptopia.co.nz/api/";

        private static readonly ObjectId IdHash = "prime:cryptopia".GetObjectIdHashCode();

        //Could not find any documentation about rate limit.
        private static readonly IRateLimiter Limiter = new NoRateLimits();

        public char? CommonPairSeparator => '_';

        private RestApiClientProvider<ICryptopiaApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Cryptopia");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        private static readonly IAssetCodeConverter AssetCodeConverter = new CryptopiaCodeConverter();

        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public CryptopiaProvider()
        {
            ApiProvider = new RestApiClientProvider<ICryptopiaApi>(CryptopiaApiUrl, this, k => new CryptopiaAuthenticator(k).GetRequestModifierAsync);
        }

        private void CheckCryptopiaResponseErrors<T>(Response<T> rRaw, [CallerMemberName] string methodName = null)
        {
            var rRawBase = rRaw.GetContent();

            var rBase = rRawBase as CryptopiaSchema.BaseResponse;

            if (rBase != null && !rBase.Success)
            {
                string error;

                if(!String.IsNullOrWhiteSpace(rBase.Error))
                    error = rBase.Error;
                else if (!String.IsNullOrWhiteSpace(rBase.Message))
                    error = rBase.Message;
                else
                    error = "Unknown API error";

                throw new ApiResponseException(error, this, methodName);
            }

            if (!rRaw.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rRaw.ResponseMessage.ReasonPhrase;
                throw new ApiResponseException(
                    $"HTTP error {rRaw.ResponseMessage.StatusCode} {(String.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}",
                    this, methodName);
            }
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetBalanceAsync(new { }).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r != null && r.Data.Length > 0;
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            return r.Success;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r.Success)
            {
                var pairs = new AssetPairs();

                foreach (var rCurrentTicker in r.Data)
                {
                    pairs.Add(rCurrentTicker.Label.ToAssetPair(this, '/'));
                }

                return pairs;
            }
            else
            {
                throw new ApiResponseException("Error processing request. " + r.Message, this);
            }
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true, CanReturnAll = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            if (context.ForSingleMethod)
                return await GetPriceAsync(context).ConfigureAwait(false);

            return await GetPricesAsync(context).ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetPriceAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this);
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r.Success)
            {
                return new MarketPrices(new MarketPrice(Network, context.Pair, r.Data.LastPrice)
                {
                    PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.Data.AskPrice, r.Data.BidPrice, r.Data.Low, r.Data.High),
                    Volume = new NetworkPairVolume(Network, context.Pair, r.Data.Volume)
                });
            }
            else
            {
                throw new ApiResponseException("Error processing request. " + r.Message, this);
            }
        }

        public async Task<MarketPrices> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickersAsync().ConfigureAwait(false);

            if (r.Success)
            {
                var prices = new MarketPrices();

                var rPairsDict = r.Data.ToDictionary(x => x.Label.ToAssetPair(this, '/'), x => x);
                var pairsQueryable = context.IsRequestAll ? rPairsDict.Keys.ToList() : context.Pairs;

                foreach (var pair in pairsQueryable)
                {
                    rPairsDict.TryGetValue(pair, out var currentTicker);

                    if (currentTicker == null)
                    {
                        prices.MissedPairs.Add(pair);
                    }
                    else
                    {
                        prices.Add(new MarketPrice(Network, pair, currentTicker.LastPrice)
                        {
                            PriceStatistics = new PriceStatistics(Network, pair.Asset2, currentTicker.AskPrice, currentTicker.BidPrice, currentTicker.Low, currentTicker.High),
                            Volume = new NetworkPairVolume(Network, pair, currentTicker.Volume)
                        });
                    }
                }

                return prices;
            }
            else
            {
                throw new ApiResponseException("Error processing request. " + r.Message, this);
            }
        }
    }
}
