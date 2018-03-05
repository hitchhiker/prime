﻿using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.TuxExchange;
using Xunit;

namespace Prime.Tests.Providers
{
    public class TuxExchangeTests : ProviderDirectTestsBase
    {
        public TuxExchangeTests()
        {
            Provider = Networks.I.Providers.OfType<TuxExchangeProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_ICN".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_ICN".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("DOGE_BTC".ToAssetPairRaw(), true);
        }
    }
}
