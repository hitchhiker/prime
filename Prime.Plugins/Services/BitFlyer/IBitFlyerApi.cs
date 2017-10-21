using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitFlyer
{
    internal interface IBitFlyerApi
    {
        [Get("/getprices")]
        Task<BitFlyerSchema.PricesResponse> GetPrices();

        [Get("/getmarkets")]
        Task<BitFlyerSchema.MarketsResponse> GetMarkets();

        [Get("/getticker?product_code={productCode}")]
        Task<BitFlyerSchema.TickerResponse> GetTicker([Path] string productCode);

        [Get("/getboard?product_code={productCode}")]
        Task<BitFlyerSchema.BoardResponse> GetBoard([Path] string productCode);
    }
}
