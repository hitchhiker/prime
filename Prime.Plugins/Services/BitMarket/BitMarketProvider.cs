﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitMarket
{
    public class BitMarketProvider : IExchangeProvider
    {
        private const string BitMarketApiUrl = "https://www.bitmarket.net/";

        private RestApiClientProvider<IBitMarketApi> ApiProvider { get; }

        private static readonly AssetPairs Pairs = new AssetPairs(new AssetPair[]
        {
            "BTC_PLN".ToAssetPairRaw(),
            "BTC_EUR".ToAssetPairRaw(),
            "LTC_PLN".ToAssetPairRaw(),
            "LTC_BTC".ToAssetPairRaw(),
            new AssetPair("LiteMineX", "BTC") 
        });

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
        private static IAssetCodeConverter AssetCodeConverter = new BitMarketCodeConverter();

        public BitMarketProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitMarketApi>(BitMarketApiUrl, this, k => null);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetBitMarketPairCode(context.Pair);

            var r = await api.GetTicker(pairCode);

            return new MarketPrice(context.Pair, r.last);
        }

        private string GetBitMarketPairCode(AssetPair pair)
        {
            var asset1 = pair.Asset1.ToRemoteCode(this);
            var asset2 = pair.Asset2.ToRemoteCode(this);

            return $"{asset1}{asset2}";
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
