using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.ShapeShift
{
    internal interface IShapeShiftApi
    {
        [Get("/rate/{pair}")]
        Task<ShapeShiftSchema.RateResponse> GetMarketInfo([Path] string pair);

        [Get("/marketinfo/")]
        Task<ShapeShiftSchema.MarketInfosResponse> GetMarketInfos();
    }
}