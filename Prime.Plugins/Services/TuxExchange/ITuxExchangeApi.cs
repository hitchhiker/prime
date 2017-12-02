using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.TuxExchange
{
    internal interface ITuxExchangeApi
    {
        [Get("/api?method=getticker")]
        Task<Dictionary<string, TuxEchangeSchema.TickerResponse>> GetTickersAsync();

        [Get("/api?method=get24hvolume")]
        Task<TuxEchangeSchema.VolumeResponse> GetVolumesAsync();
    }
}
