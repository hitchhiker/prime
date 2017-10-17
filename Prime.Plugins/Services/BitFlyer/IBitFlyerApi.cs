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
    }
}
