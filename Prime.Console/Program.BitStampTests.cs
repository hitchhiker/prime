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
                    var price = AsyncContext.Run(() => provider.GetPricingAsync(priceContext));

                    System.Console.WriteLine($"Latest {pair}: {price.FirstPrice}");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
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
		                System.Console.WriteLine($"{result.Asset}: {result.AvailableAndReserved}, {result.Available}, {result.Reserved}");
		            }
		        }
		        catch (Exception e)
		        {
		            System.Console.WriteLine(e.Message);
		            throw;
		        }
            }

		    public void GetAssetPairs()
		    {
		        var provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();
		        var ctx = new NetworkProviderContext();

		        try
		        {
		            var pairs = AsyncContext.Run(() => provider.GetAssetPairsAsync(ctx));

		            foreach (var pair in pairs)
		            {
		                System.Console.WriteLine($"{pair}");
		            }
		        }
		        catch (Exception e)
		        {
		            System.Console.WriteLine(e);
		            throw;
		        }
            }

		    public void GetDepositAddresses()
		    {
		        var provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();

		        var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), UserContext.Current);
		        var ctxAll = new WalletAddressContext(UserContext.Current);

                try
		        {
		            var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));
		            var addressesAll = AsyncContext.Run(() => provider.GetAddressesAsync(ctxAll));

                    System.Console.WriteLine("Addresses for 1 asset");
		            foreach (var address in addresses)
		            {
		                System.Console.WriteLine($"{address.Asset} : {address.Address}");
		            }

		            System.Console.WriteLine("Addresses for all assets");
		            foreach (var address in addressesAll)
		            {
		                System.Console.WriteLine($"{address.Asset} : {address.Address}");
		            }
		        }
		        catch (Exception e)
		        {
		            System.Console.WriteLine(e);
		            throw;
		        }
            }
		}
    }
}
