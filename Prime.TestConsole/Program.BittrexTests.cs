using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Core;
using Prime.Plugins.Services.Bittrex;
using Prime.Plugins.Services.Poloniex;

namespace Prime.TestConsole
{
    public partial class Program
    {
		public class BittrexTests
		{
		    public void ApiTest()
		    {
		        var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
		        var apiTestCtx = new ApiTestContext(UserContext.Current.GetApiKey(provider));

		        var ok = AsyncContext.Run(() => provider.TestApiAsync(apiTestCtx));


                try
                {

		            Console.WriteLine($"Api test OK: {ok}");
		        }
		        catch (Exception e)
		        {
		            Console.WriteLine(e.Message);
		            throw;
		        }
            }

		    public void GetDepositAddresses()
		    {
		        var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();

		        var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), true, UserContext.Current);
		        var ctxAll = new WalletAddressContext(false, UserContext.Current);

		        var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));
		        var addressesAll = AsyncContext.Run(() => provider.GetAddressesAsync(ctxAll));

                try
		        {
		            Console.WriteLine("Addresses for 1 asset");
		            foreach (var address in addresses)
		            {
		                Console.WriteLine($"{address.Asset} : {address.Address}");
		            }

                    Console.WriteLine("Addresses for all assets");
                    foreach (var address in addressesAll)
                    {
                        Console.WriteLine($"{address.Asset} : {address.Address}");
                    }
                }
		        catch (Exception e)
		        {
		            Console.WriteLine(e);
		            throw;
		        }
            }

		    public void GetAssetPairs()
		    {
		        var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
		        var ctx = new NetworkProviderContext();

		        try
		        {
		            var pairs = AsyncContext.Run(() => provider.GetAssetPairs(ctx));

		            foreach (var pair in pairs)
		            {
		                Console.WriteLine($"{pair}");
		            }
		        }
		        catch (Exception e)
		        {
		            Console.WriteLine(e.Message);
		            throw;
		        }
            }
		}
    }
}
