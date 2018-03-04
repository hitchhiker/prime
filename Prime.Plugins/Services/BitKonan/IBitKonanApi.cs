using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitKonan
{
    [AllowAnyStatusCode]
    internal interface IBitKonanApi
    {
        [Get("/ticker")]
        Task<BitKonanSchema.TickerResponse> GetBtcTickerAsync();

        [Get("/ltc_ticker")]
        Task<BitKonanSchema.TickerResponse> GetLtcTickerAsync();

        [Get("/{asset}_orderbook")]
        Task<BitKonanSchema.OrderBookResponse> GetOrderBookAsync([Path] string asset);

        [Post("/private/balance")]
        Task<BitKonanSchema.BalanceResponse> GetBalanceAsync();

        [Post("/private/order/new")]
        Task<BitKonanSchema.NewOrderResponse> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
        
        [Post("/private/orders")]
        Task<BitKonanSchema.OrderInfoResponse> QueryActiveOrdersAsync();
    }
}
