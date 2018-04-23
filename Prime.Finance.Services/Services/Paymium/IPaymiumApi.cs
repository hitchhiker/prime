using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Paymium
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
