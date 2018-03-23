using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Xbtce
{
    [Header("Content-Type", "application/json")]
    [Header("Accept", "application/json")]
    [Header("Accept-Encoding", "gzip")]
    [AllowAnyStatusCode]
    internal interface IXbtceApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<XbtceSchema.TickerResponse[]> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<XbtceSchema.TickerResponse[]> GetTickersAsync();

        [Get("/account")]
        Task<Response<XbtceSchema.UserInfoResponse>> GetUserInfoAsync();

        [Post("/trade")]
        Task<Response<XbtceSchema.OrderInfoResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
        
        [Get("/trade/{id}")]
        Task<Response<XbtceSchema.OrderInfoResponse>> QueryOrderAsync([Path] string id);

    }
}
