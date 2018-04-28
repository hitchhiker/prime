﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;

namespace Prime.Finance.Services.Services.Coinone
{
    // http://doc.coinone.co.kr/#api-Public
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class CoinoneProvider : IAssetPairsProvider, IPublicPricingProvider
    {
        private const string CoinoneApiUrl = "https://api.coinone.co.kr";

        private static readonly ObjectId IdHash = "prime:coinone".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Coinone");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;

        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(90, 1);
        public IRateLimiter RateLimiter => Limiter;
        public char? CommonPairSeparator { get; }

        private RestApiClientProvider<ICoinoneApi> ApiProvider { get; }

        public CoinoneProvider()
        {
            ApiProvider = new RestApiClientProvider<ICoinoneApi>(CoinoneApiUrl, this, k => null);
        }
        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }
        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTickersAsync().ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = ParseTicker(rRaw);

            var pairs = new AssetPairs();
            var krwAsset = Asset.Krw;

            foreach (var kvp in r)
            {
                var pair = kvp.Key.ToAsset(this).ToPair(krwAsset);
                pairs.Add(pair);
            }

            return pairs;
        }

        private Dictionary<string, CoinoneSchema.TickerEntryResponse> ParseTicker(CoinoneSchema.TickersResponse rRaw)
        {
            var dict = new Dictionary<string, CoinoneSchema.TickerEntryResponse>();
            var keywords = new string[]
            {
                "result",
                "errorCode",
                "timestamp"
            };

            foreach (var kvp in rRaw)
            {
                if (keywords.Contains(kvp.Key))
                    continue;

                var ticker = JsonConvert.DeserializeObject<CoinoneSchema.TickerEntryResponse>(kvp.Value.ToString());
                dict.Add(kvp.Key, ticker);
            }

            return dict;
        }

        private void CheckResponseErrors(Dictionary<string, object> dictResponse)
        {
            object result = String.Empty;
            object errorCode = String.Empty;

            if (!dictResponse.TryGetValue("result", out result))
                throw new FormatException("Dictionary does not contain `result` key");
            if (!dictResponse.TryGetValue("errorCode", out errorCode))
                throw new FormatException("Dictionary does not contain `errorCode` key");

            var baseResponse = new CoinoneSchema.BaseResponse()
            {
                result = result.ToString(),
                errorCode = errorCode.ToString()
            };

            CheckResponseErrors(baseResponse);
        }

        private void CheckResponseErrors(CoinoneSchema.BaseResponse baseResponse)
        {
            if (!baseResponse.result.Equals("success") && !baseResponse.errorCode.Equals("0"))
                throw new ApiResponseException($"Error {baseResponse.errorCode}", this);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (!context.Pair.Asset2.ToRemoteCode(this).Equals(Asset.Krw.ShortCode))
                throw new AssetPairNotSupportedException(context.Pair, this);

            var r = await api.GetTickerAsync(context.Pair.Asset1.ShortCode).ConfigureAwait(false);

            CheckResponseErrors(r);

            return new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.QuoteAsset, null, null, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            };
        }

        public Task<MarketPrices> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricingAsync(context);
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures(false, true);
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetTickersAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = ParseTicker(rRaw);

            var prices = new MarketPrices();
            var krwAsset = Asset.Krw;

            foreach (var pair in context.Pairs)
            {
                var ticker = new CoinoneSchema.TickerEntryResponse();
                if (!r.TryGetValue(pair.Asset1.ShortCode.ToLower(), out ticker) || !pair.Asset2.Equals(krwAsset))
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.Add(new MarketPrice(Network, pair, ticker.last));
            }

            return prices;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (!context.Pair.Asset2.Equals(Asset.Krw))
                throw new AssetPairNotSupportedException(context.Pair, this);

            var r = await api.GetTickerAsync(context.Pair.Asset1.ShortCode).ConfigureAwait(false);

            CheckResponseErrors(r);

            return new PublicVolumeResponse(Network, context.Pair, r.volume);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}
