using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Core;
using Prime.Finance;
using Prime.Finance.Services.Services.Coinbase;

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
                    var price = AsyncContext.Run(() => provider.GetPricingAsync(ctx));

                    System.Console.WriteLine($"Latest price for {pair} is {price.FirstPrice.Price}");
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
