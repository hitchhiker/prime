using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Plugins.Services.Kraken;
using Prime.Plugins.Services.Poloniex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class PoloniexTests : ProviderDirectTestsBase
    {
        public PoloniexTests()
        {
            Provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
        }
    }
}
