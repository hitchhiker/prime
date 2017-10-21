using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Common;
using Prime.Plugins.Services.Bittrex;
using Prime.Plugins.Services.Coinbase;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class CoinbaseTests
        {
            public void LatestPrice()
            {
                var provider = Networks.I.Providers.OfType<CoinbaseProvider>().FirstProvider();
                var pair = new AssetPair("BTC", "USd");

                var ctx = new PublicPriceContext(pair);

                try
                {
                    var price = AsyncContext.Run(() => provider.GetLatestPriceAsync(ctx));

                    Console.WriteLine($"Latest price for {pair} is {price.Price}");
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
