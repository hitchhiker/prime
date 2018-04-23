using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.CoinCorner
{
    internal interface ICoinCornerApi
    {
        [Get("/Ticker?Coin={Coin}&Currency={Currency}")]
        Task<CoinCornerSchema.TickerResponse> GetTickerAsync([Path] string Coin, [Path] string Currency);

        [Get("/OrderBook?Coin={Coin}")]
        Task<CoinCornerSchema.OrderBookResponse> GetOrderBookAsync([Path] string Coin);
    }
}
