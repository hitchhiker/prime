using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.EthexIndia
{
    internal interface IEthexIndiaApi
    {
        [Get("/ticker/")]
        Task<EthexIndiaSchema.TickerResponse[]> GetTickersAsync();

        [Get("/order_book/")]
        Task<EthexIndiaSchema.OrderBookResponse> GetOrderBookAsync();
    }
}
