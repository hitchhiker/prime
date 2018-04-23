using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.BitFlyer
{
    internal interface IBitFlyerApi
    {
        [Get("/getprices")]
        Task<BitFlyerSchema.PricesResponse> GetPricesAsync();

        [Get("/getmarkets")]
        Task<BitFlyerSchema.MarketsResponse> GetMarketsAsync();

        [Get("/getticker?product_code={productCode}")]
        Task<BitFlyerSchema.TickerResponse> GetTickerAsync([Path] string productCode);

        [Get("/getboard?product_code={productCode}")]
        Task<BitFlyerSchema.BoardResponse> GetBoardAsync([Path] string productCode);
    }
}
