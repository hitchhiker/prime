using System;
using System.Linq;
using Nito.AsyncEx;
using Prime.Core;
using Prime.Plugins.Services.BitStamp;

namespace Prime.TestConsole
{
    public partial class Program
    {
		public class BitStampTests
		{
		    public void GetTicker()
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
		            var balance = AsyncContext.Run(() => provider.GetBalancesAsync(privateContext));

		            foreach (var result in balance)
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
		}
    }
}
