using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Ccex
{
    [AllowAnyStatusCode]
    internal interface ICcexApi
    {
        [Get("/t/{currencyPair}.json")]
        Task<CcexSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/t/prices.json")]
        Task<CcexSchema.AllTickersResponse> GetTickersAsync();

        [Get("/t/pairs.json")]
        Task<CcexSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/t/api_pub.html?a=getmarketsummaries")]
        Task<CcexSchema.VolumeResponse> GetVolumesAsync();

        [Get("/t/api_pub.html?a=getorderbook&market={currencyPair}&type=both&depth={depth}")]
        Task<CcexSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string currencyPair, [Path] int depth);

        [Get("/t/api.html?a=getbalances")]
        Task<Response<CcexSchema.BalancesResponse>> GetBalancesAsync();

        [Get("/t/api.html?a=getorder&uuid={uuid}")]
        Task<Response<CcexSchema.OrderInfoResponse[]>> QueryOrderAsync([Path] string uuid);

        [Get("/t/api.html?a=buylimit&market={market}&quantity={quantity}&rate={rate}")]
        Task<Response<CcexSchema.OrderLimitResponse>> PlaceBuyOrderAsync([Path(UrlEncode = false)] string market, [Path] decimal quantity, [Path] decimal rate);

        [Get("/t/api.html?a=selllimit&market={market}&quantity={quantity}&rate={rate}")]
        Task<Response<CcexSchema.OrderLimitResponse>> PlaceSellOrderAsync([Path(UrlEncode = false)] string market, [Path] decimal quantity, [Path] decimal rate);
    }
}
