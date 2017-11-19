﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitFlyer;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BitFlyerTests : ProviderDirectTestsBase
    {
        public BitFlyerTests()
        {
            Provider = Networks.I.Providers.OfType<BitFlyerProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        { 
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "BTC_JPY".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(context);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()));
            base.TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()), 20);
            base.TestGetOrderBook(context);
        }
    }
}
