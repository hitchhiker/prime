using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.ItBit
{
    internal interface IItBitApi
    {
        [Get("/markets/{pairTicker}/ticker")]
        Task<ItBitSchema.TickerResponse> GetTickerAsync([Path] string pairTicker);
    }
}
