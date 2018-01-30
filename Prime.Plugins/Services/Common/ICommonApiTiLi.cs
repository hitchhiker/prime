using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Plugins.Services.Base;
using RestEase;

namespace Prime.Plugins.Services.Common
{
    /// <summary>
    /// Contains API methods for Liqui and Tidex exchanges.
    /// </summary>
    public interface ICommonApiTiLi
    {
        [Get("/ticker/{pairsCsv}")]
        Task<Dictionary<string, CommonSchemaTiLi.TickerData>> GetTickerAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/depth/{pairsCsv}")]
        Task<CommonSchemaTiLi.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/info")]
        Task<CommonSchemaTiLi.AssetPairsResponse> GetAssetPairsAsync();

        [Post("/")]
        Task<CommonSchemaTiLi.UserInfoExtResponse> GetUserInfoExtAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<CommonSchemaTiLi.UserInfoResponse> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<CommonSchemaTiLi.OrderInfoResponse> GetOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<CommonSchemaTiLi.TradeResponse> TradeAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}