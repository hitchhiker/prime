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

        [Get("/{asset}_orderbook")]
        Task<BitKonanSchema.OrderBookResponse> GetOrderBookAsync([Path] string asset);

        //[Get("/ltc_orderbook")]
        //Task<BitKonanSchema.OrderBookLtcResponse> GetLtcOrderBookAsync();
    }
}
