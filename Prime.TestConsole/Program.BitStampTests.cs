using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Core;

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
		}
    }
}
