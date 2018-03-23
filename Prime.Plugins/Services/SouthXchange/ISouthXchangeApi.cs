using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.SouthXchange
{
    [AllowAnyStatusCode]
    internal interface ISouthXchangeApi
    {
        [Get("/price/{currencyPair}")]
        Task<SouthXchangeSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/prices")]
        Task<SouthXchangeSchema.AllTickerResponse[]> GetTickersAsync();

        [Get("/book/{currencyPair}")]
        Task<SouthXchangeSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/listBalances")]
        Task<Response<SouthXchangeSchema.BalanceResponse[]>> GetBalancesAsync();

        [Post("/placeOrder")]
        Task<Response<string>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/listOrders")]
        Task<Response<SouthXchangeSchema.OrderInfoResponse[]>> QueryOrdersAsync();

        [Post("/withdraw")]
        Task<Response<object>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
    }
}
