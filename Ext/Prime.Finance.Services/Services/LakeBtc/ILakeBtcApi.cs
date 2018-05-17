using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.LakeBtc
{
    internal interface ILakeBtcApi
    {
        [Get("/ticker")]
        Task<LakeBtcSchema.AllTickersResponse> GetTickersAsync();

        [Get("/bcorderbook?symbol={currencyPair}")]
        Task<LakeBtcSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/")]
        Task<object> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
