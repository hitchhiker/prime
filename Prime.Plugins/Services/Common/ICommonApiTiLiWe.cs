using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Plugins.Services.Base;
using RestEase;

namespace Prime.Plugins.Services.Common
{
    /// <summary>
    /// Contains API methods for Liqui and Tidex exchanges.
    /// </summary>
    public interface ICommonApiTiLiWe
    {
        [Get("/ticker/{pairsCsv}")]
        Task<Dictionary<string, CommonSchemaTiLiWe.TickerData>> GetTickerAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/depth/{pairsCsv}")]
        Task<CommonSchemaTiLiWe.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/info")]
        Task<CommonSchemaTiLiWe.AssetPairsResponse> GetAssetPairsAsync();

        [Post("/")]
        Task<CommonSchemaTiLiWe.UserInfoExtResponse> GetUserInfoExtAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<CommonSchemaTiLiWe.UserInfoResponse> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<CommonSchemaTiLiWe.OrderInfoResponse> GetOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<CommonSchemaTiLiWe.TradeResponse> TradeAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}