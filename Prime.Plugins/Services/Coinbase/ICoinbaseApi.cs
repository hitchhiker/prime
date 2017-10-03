#region

using System.Threading.Tasks;
using plugins.Services.Coinbase;
using RestEase;

#endregion

namespace plugins
{
    internal interface ICoinbaseApi
    {
        [Get("accounts")]
        Task<CoinbaseSchema.AccountsResponse> GetAccountsAsync();

        [Get("/accounts/{account_id}/addresses")]
        Task<CoinbaseSchema.WalletAddressesResponse> GetAddressesAsync([Path("account_id")] string accountId);

        [Get("/accounts/{account_id}/addresses/{address_id}")]
        Task<CoinbaseSchema.WalletAddressResponse> GetAddressAsync([Path("account_id")] string accountId, [Path("address_id")] string addressId);

        [Get("/accounts/{account_id}/addresses")]
        Task<CoinbaseSchema.CreateWalletAddressResponse> CreateAddressAsync([Path("account_id")] string accountId);

        [Get("/accounts")]
        Task<CoinbaseSchema.AccountsResponse> GetAccounts();

        [Get("/prices/{currencyPair}/spot")]
        Task<CoinbaseSchema.SpotPriceResponse> GetLatestPrice([Path] string currencyPair);
    }
}