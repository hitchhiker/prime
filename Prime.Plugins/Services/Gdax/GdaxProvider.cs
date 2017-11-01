﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Gdax
{
    public class GdaxProvider : IExchangeProvider
    {
        private const string GdaxApiUrl = "https://api.gdax.com";

        private static readonly ObjectId IdHash = "prime:gdax".GetObjectIdHashCode();

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("GDAX");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        /// <summary>
        /// We throttle public endpoints by IP: 3 requests per second, up to 6 requests per second in bursts.
        /// We throttle private endpoints by user ID: 5 requests per second, up to 10 requests per second in bursts.
        /// See https://docs.gdax.com/#rate-limits.
        /// </summary>
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(180, 1, 300, 1);

        public IRateLimiter RateLimiter => Limiter;

        private RestApiClientProvider<IGdaxApi> ApiProvider { get; }

        public GdaxProvider()
        {
            ApiProvider = new RestApiClientProvider<IGdaxApi>(GdaxApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetProducts();

            var pairs = new AssetPairs();

            foreach (var rProduct in r)
            {
                pairs.Add(new AssetPair(rProduct.base_currency, rProduct.quote_currency));
            }

            return pairs;
        }

        public async Task<LatestPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerDash();
            var r = await api.GetProductTicker(pairCode);

            var price = new LatestPrice()
            {
                UtcCreated = DateTime.UtcNow,
                Price = new Money(r.price, context.Pair.Asset2),
                QuoteAsset = context.Pair.Asset1
            };

            return price;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
