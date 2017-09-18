using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using plugins;
using Prime.Core;
using Prime.Plugins.Services.BitMex;

namespace Prime.TestConsole
{
    class Worker
    {
        public void Run()
        {
            UserContext userContext = new UserContext(ObjectId.NewObjectId(), "Alex");
            ApiTestContext testContext = new ApiTestContext(new ApiKey("Key", BitMexAuthenticator.Key, BitMexAuthenticator.Secret));

            NetworkProviderPrivateContext providerPrivateContext = new NetworkProviderPrivateContext(userContext);

            BitMexProvider provider = new BitMexProvider();

            IWalletService walletService = provider;
            //var balances = walletService.GetBalancesAsync(providerPrivateContext).Result;

            IPublicPriceProvider priceProvider = provider;

            PublicPriceContext priceContext = new PublicPriceContext(new AssetPair("XBT".ToAsset(provider), "USD".ToAsset(provider)));

            var latestPrice = priceProvider.GetLatestPriceAsync(priceContext).Result;

            //walletService.TestApiAsync(testContext).RunSynchronously();
        }
    }
}
