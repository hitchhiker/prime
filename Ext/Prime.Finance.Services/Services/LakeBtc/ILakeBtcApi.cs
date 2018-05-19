using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.LakeBtc
{
    [AllowAnyStatusCode]
    internal interface ILakeBtcApi
    {
        [Get("/ticker")]
        Task<LakeBtcSchema.AllTickersResponse> GetTickersAsync();

        [Get("/bcorderbook?symbol={currencyPair}")]
        Task<LakeBtcSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/")]
        Task<Response<LakeBtcSchema.UserInfoResponse>> GetUserInfoAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<LakeBtcSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<LakeBtcSchema.OrderResponse>> QueryOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/")]
        Task<Response<LakeBtcSchema.WithdrawalResponse>> PlaceWithdrawalAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
    }
}
