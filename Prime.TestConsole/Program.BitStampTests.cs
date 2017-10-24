using System;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Plugins.Services.BitStamp;

namespace Prime.TestConsole
{
    public partial class Program
    {
		public class BitStampTests
		{
		    public void GetLatestPrices()
		    {
                var provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();
		        var pair = new AssetPair("BTC", "USD");

                var priceContext = new PublicPriceContext(pair);

                try
                {
                    var price = AsyncContext.Run(() => provider.GetLatestPriceAsync(priceContext));

                    Console.WriteLine($"Latest {pair}: {price.Price}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

		    public void GetAccountBalance()
		    {
		        var provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();

		        var privateContext = new NetworkProviderPrivateContext(UserContext.Current);

                try
                {
                    var balances = AsyncContext.Run(() => provider.GetBalancesAsync(privateContext));

                    foreach (var result in balances)
		            {
		                Console.WriteLine($"{result.Asset}: {result.Balance}, {result.Available}, {result.Reserved}");
		            }
		        }
		        catch (Exception e)
		        {
		            Console.WriteLine(e.Message);
		            throw;
		        }
            }

		    public void GetAssetPairs()
		    {
		        var provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();
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
		            Console.WriteLine(e);
		            throw;
		        }
            }

		    public void GetDepositAddresses()
		    {
		        var provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();

		        var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), false, UserContext.Current);
		        var ctxAll = new WalletAddressContext(UserContext.Current);

                try
		        {
		            var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));
		            var addressesAll = AsyncContext.Run(() => provider.GetAddressesAsync(ctxAll));

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
		}
    }
}
