using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinbase
{
    [AllowAnyStatusCode]
    internal interface ICoinbaseApi
    {
        [Get("/accounts")]
        Task<Response<CoinbaseSchema.AccountsResponse>> GetAccountsAsync();

        [Get("/accounts/{accountId}/addresses")]
        Task<CoinbaseSchema.WalletAddressesResponse> GetAddressesAsync([Path] string accountId);

        [Get("/accounts/{accountId}/addresses/{address_id}")]
        Task<CoinbaseSchema.WalletAddressResponse> GetAddressAsync([Path] string accountId, [Path("address_id")] string addressId);

        [Get("/accounts/{accountId}/addresses")]
        Task<CoinbaseSchema.CreateWalletAddressResponse> CreateAddressAsync([Path] string accountId);

        [Post("/accounts/{accountId}/buys")]
        Task<Response<CoinbaseSchema.PlaceOrderResponse>> PlaceBuyOrderAsync([Path] string accountId, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/accounts/{accountId}/sells")]
        Task<Response<CoinbaseSchema.PlaceOrderResponse>> PlaceSellOrderAsync([Path] string accountId, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Get("/accounts/{accountId}/sells")]
        Task<Response<CoinbaseSchema.OrdersListResponse>> ListSellsAsync([Path] string accountId);

        [Get("/accounts/{accountId}/buys")]
        Task<Response<CoinbaseSchema.OrdersListResponse>> ListBuysAsync([Path] string accountId);

        [Get("/accounts/{accountId}/buys/{buyId}")]
        Task<Response<CoinbaseSchema.ShowOrderResponse>> ShowBuyOrder([Path] string accountId, [Path] string buyId);

        [Get("/accounts/{accountId}/sells/{buyId}")]
        Task<Response<CoinbaseSchema.ShowOrderResponse>> ShowSellOrder([Path] string accountId, [Path] string buyId);

        [Get("/payment-methods")]
        Task<Response<CoinbaseSchema.PaymentMethods>> GetPaymentMethodsAsync();

        [Get("/prices/{currencyPair}/spot")]
        Task<CoinbaseSchema.SpotPriceResponse> GetLatestPriceAsync([Path] string currencyPair);

        [Get("/time")]
        Task<CoinbaseSchema.TimeResponse> GetCurrentServerTimeAsync();
    }
}