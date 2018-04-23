using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.StocksExchange
{
    internal interface IStocksExchangeApi
    {
        [Get("/ticker")]
        Task<StocksExchangeSchema.AllTickersResponse[]> GetTickersAsync();
    }
}
