using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinbase
{
    [AllowAnyStatusCode]
    internal interface ICoinbaseApi
    {
        [Get("/accounts")]
        Task<CoinbaseSchema.AccountsResponse> GetAccountsAsync();

        [Get("/accounts/{account_id}/addresses")]
        Task<CoinbaseSchema.WalletAddressesResponse> GetAddressesAsync([Path("account_id")] string accountId);

        [Get("/accounts/{account_id}/addresses/{address_id}")]
        Task<CoinbaseSchema.WalletAddressResponse> GetAddressAsync([Path("account_id")] string accountId, [Path("address_id")] string addressId);

        [Get("/accounts/{account_id}/addresses")]
        Task<CoinbaseSchema.CreateWalletAddressResponse> CreateAddressAsync([Path("account_id")] string accountId);

        [Post("/accounts/{account_id}/buys")]
        Task<Response<CoinbaseSchema.PlaceBuyOrderResponse>> PlaceBuyOrderAsync([Path("account_id")] string accountId, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/accounts/{account_id}/sells")]
        Task<Response<CoinbaseSchema.PlaceBuyOrderResponse>> PlaceSellOrderAsync([Path("account_id")] string accountId, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Get("/payment-methods")]
        Task<Response<CoinbaseSchema.PaymentMethods>> GetPaymentMethodsAsync();

        [Get("/prices/{currencyPair}/spot")]
        Task<CoinbaseSchema.SpotPriceResponse> GetLatestPriceAsync([Path] string currencyPair);

        [Get("/time")]
        Task<CoinbaseSchema.TimeResponse> GetCurrentServerTimeAsync();
    }
}