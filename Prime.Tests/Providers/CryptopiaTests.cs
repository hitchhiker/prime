using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Cryptopia;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class CryptopiaTests : ProviderDirectTestsBase
    {
        public CryptopiaTests()
        {
            Provider = Networks.I.Providers.OfType<CryptopiaProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestPlaceOrderLimit()
        {
            base.TestPlaceOrderLimit("ETH_BTC".ToAssetPairRaw(), false, 1, new Money(100000, Asset.Btc));
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            base.TestGetTradeOrderStatus("1234", "DOT_BTC".ToAssetPairRaw());
        }

        [TestMethod]
        public override void TestPlaceWithdrawalExtended()
        {
            base.TestPlaceWithdrawalExtended(new WithdrawalPlacementContextExtended(UserContext.Current)
            {
                Address = new WalletAddress("12498176298371628471628376"),
                Amount = new Money(1, Asset.Btc)
            });
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BCH_USDT".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BCH_USDT".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
