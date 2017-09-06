#region

using System.Collections.Generic;
using System.Threading.Tasks;
using plugins.Services.Coinbase;
using plugins.Services.CryptoCompare.Model;
using RestEase;

#endregion

namespace plugins
{
    public interface ICoinbaseApi
    {
        [Get("accounts")]
        Task<Accounts> GetAccountsAsync();

        [Get("/accounts/{account_id}/addresses")]
        Task<WalletAddresses> GetAddressesAsync([Path("account_id")] string accountId);

        [Get("/accounts/{account_id}/addresses/{address_id}")]
        Task<WalletAddress> GetAddressAsync([Path("account_id")] string accountId, [Path("address_id")] string addressId);

        [Get("/accounts/{account_id}/addresses")]
        Task<CreateWalletAddress> CreateAddressAsync([Path("account_id")] string accountId);

        [Get("/accounts")]
        Task<Accounts> GetAccounts();
    }
}