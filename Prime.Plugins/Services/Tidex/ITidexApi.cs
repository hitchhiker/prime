using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Tidex
{
    internal interface ITidexApi
    {
        [Get("/ticker/{pairsCsv}")]
        Task<Dictionary<string, TidexSchema.TickerData>> GetTickerAsync([Path(UrlEncode = false)] string pairsCsv);

        [Get("/info")]
        Task<TidexSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Post("/")]
        Task<TidexSchema.UserInfoExtResponse> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/")]
        Task<TidexSchema.OrderInfoResponse> GetOrderInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
