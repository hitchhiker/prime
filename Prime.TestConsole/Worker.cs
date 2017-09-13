using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using plugins;
using Prime.Core;

namespace Prime.TestConsole
{
    class Worker
    {
        public void Run()
        {
            UserContext userContext = new UserContext(ObjectId.NewObjectId(), "Alex");

            #region Secrets

            ApiTestContext testContext = new ApiTestContext(new ApiKey("Key", "KEY", "SECRET"));

            #endregion

            NetworkProviderPrivateContext providerPrivateContext = new NetworkProviderPrivateContext(userContext);

            IWalletService walletService = new BitMexProvider();
            //var results = walletService.GetBalance(providerPrivateContext);
            walletService.TestApi(testContext).RunSynchronously();
        }
    }
}
