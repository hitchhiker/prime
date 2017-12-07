using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitcoinIndonesia
{
    internal interface IBitcoinIndonesiaApi
    {
        [Get("/{currencyPair}/ticker")]
        Task<BitcoinIndonesiaSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
