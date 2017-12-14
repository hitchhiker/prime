using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitKonan
{
    internal interface IBitKonanApi
    {
        [Get("/ticker")]
        Task<BitKonanSchema.TickerResponse> GetBtcTickerAsync();

        [Get("/ltc_ticker")]
        Task<BitKonanSchema.TickerResponse> GetLtcTickerAsync();
    }
}
