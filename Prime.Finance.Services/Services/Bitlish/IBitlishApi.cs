using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Bitlish
{
    internal interface IBitlishApi
    {
        [Get("/tickers")]
        Task<BitlishSchema.AllTickersResponse> GetTickersAsync();

        [Get("/trades_depth?pair_id={currencyPair}")]
        Task<BitlishSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
