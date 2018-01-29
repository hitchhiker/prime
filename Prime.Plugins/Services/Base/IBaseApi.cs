using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Plugins.Services.Tidex;
using RestEase;

namespace Prime.Plugins.Services.Base
{
    public interface IBaseApi
    {
        [Get("/ticker/{pairsCsv}")]
        Task<Dictionary<string, BaseSchema.TickerData>> GetTickerAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/depth/{pairsCsv}")]
        Task<BaseSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/info")]
        Task<BaseSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Post("/")]
        Task<BaseSchema.UserInfoExtResponse> GetUserInfoExtAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<BaseSchema.UserInfoResponse> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<BaseSchema.OrderInfoResponse> GetOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<BaseSchema.TradeResponse> TradeAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}