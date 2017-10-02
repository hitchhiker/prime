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
		}
    }
}
