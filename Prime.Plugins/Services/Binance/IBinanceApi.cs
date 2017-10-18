using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Binance
{
    internal interface IBinanceApi
    {
        [Get("/ticker/allPrices")]
        Task<BinanceSchema.LatestPricesResponse> GetSymbolPriceTicker();
    }
}
