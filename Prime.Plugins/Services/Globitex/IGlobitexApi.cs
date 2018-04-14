using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Globitex
{
    [AllowAnyStatusCode]
    internal interface IGlobitexApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<GlobitexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<GlobitexSchema.AllTickersResponse> GetTickersAsync();

        [Get("/public/symbols")]
        Task<GlobitexSchema.SymbolsResponse> GetSymbolsAsync();

        [Get("/public/time")]
        Task<GlobitexSchema.TimeResponse> GetTimeAsync();

        [Get("/public/orderbook/{currencyPair}")]
        Task<GlobitexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/payment/accounts")]
        Task<Response<GlobitexSchema.BalanceResponse>> GetBalanceAsync();

        [Post("/trading/new_order")]
        Task<Response<GlobitexSchema.NewOrderResponse>> PlaceNewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
        
        [Get("/trading/orders/active")]
        Task<Response<GlobitexSchema.ActiveOrdersResponse>> GetActiveOrdersAsync();

        [Get("/trading/trades")]
        Task<Response<GlobitexSchema.MyTradesResponse>> GetMyTradesAsync();
    }
}
