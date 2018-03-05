﻿using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.QuadrigaCX;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class QuadrigaCxTests : ProviderDirectTestsBase
    {
        public QuadrigaCxTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<QuadrigaCxProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "BTC_CAD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_CAD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_CAD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_CAD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
