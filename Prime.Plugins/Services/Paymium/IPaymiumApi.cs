using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Paymium
{
    internal interface IPaymiumApi
    {
        [Get("/data/{currency}/ticker")]
        Task<PaymiumSchema.TickerResponse> GetTickerAsync([Path] string currency);

        [Get("/countries")]
        Task<PaymiumSchema.CountriesResponse[]> GetCountriesAsync();

        [Get("/data/{currency}/depth")]
        Task<PaymiumSchema.OrderBookResponse> GetOrderBookAsync([Path] string currency);
    }
}
