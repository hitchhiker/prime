﻿using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.BTCXIndia;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BtcxIndiaTests : ProviderDirectTestsBase
    {
        public BtcxIndiaTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BtcxIndiaProvider>().FirstProvider();
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
                "BTC_INR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_INR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
