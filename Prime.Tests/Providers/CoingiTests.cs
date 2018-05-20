using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Core;
using Prime.Finance;
using Prime.Finance.Services.Services.Coingi;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CoingiTests : ProviderDirectTestsBase
    {
        public CoingiTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CoingiProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
