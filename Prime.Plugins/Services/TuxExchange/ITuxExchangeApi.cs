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
        Task<TuxEchangeSchema.AllTickersResponse> GetTickersAsync();

        [Get("/api?method=get24hvolume")]
        Task<TuxEchangeSchema.VolumeResponse> GetVolumesAsync();

        [Get("/api?method=getorders&coin={coin}&market={market}")]
        Task<TuxEchangeSchema.OrderBookResponse> GetOrderBookAsync([Path] string coin, [Path] string market);
    }
}
