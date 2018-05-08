using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinbase
{
    [AllowAnyStatusCode]
    internal interface ICoinbaseApi
    {
        /// <summary>
        /// Lists current user’s accounts to which the authentication method has access to.
        /// </summary>
        /// <returns></returns>
        [Get("/accounts")]
        Task<Response<CoinbaseSchema.AccountsResponse>> GetAccountsAsync();

        /// <summary>
        /// Lists addresses for an account. 
        /// Important: Addresses should be considered one time use only. 
        /// Please visit POST /accounts/:id/addresses/ for instructions on how to create new addresses.
        /// </summary>
        /// <param name="accountId">The account which addresses should be returned.</param>
        /// <returns>Addresses for specified account.</returns>
        [Get("/accounts/{accountId}/addresses")]
        Task<CoinbaseSchema.WalletAddressesResponse> GetAddressesAsync([Path] string accountId);

        /// <summary>
        /// Show an individual address for an account. 
        /// A regular bitcoin, bitcoin cash, litecoin or ethereum address can be used in place of address_id but the address has to be associated to the correct account.
        /// Important: Addresses should be considered one time use only. Please visit POST /accounts/:id/addresses/ for instructions on how to create new addresses.
        /// </summary>
        /// <param name="accountId">The account which address should be returned.</param>
        /// <param name="addressId">The id of address which should be returned.</param>
        /// <returns>Account information.</returns>
        [Get("/accounts/{accountId}/addresses/{address_id}")]
        Task<CoinbaseSchema.WalletAddressResponse> GetAddressAsync([Path] string accountId, [Path("address_id")] string addressId);

        /// <summary>
        /// Creates a new address for an account. As all the arguments are optional, it’s possible just to do a empty POST which will create a new address. 
        /// This is handy if you need to create new receive addresses for an account on-demand.
        /// </summary>
        /// <param name="accountId">The account id of user.</param>
        /// <returns>Created address information.</returns>
        [Get("/accounts/{accountId}/addresses")]
        Task<CoinbaseSchema.CreateWalletAddressResponse> CreateAddressAsync([Path] string accountId);

        /// <summary>
        /// Buys a user-defined amount of bitcoin, bitcoin cash, litecoin or ethereum.
        /// </summary>
        /// <param name="accountId">The account id of user.</param>
        /// <param name="body">Post body.</param>
        /// <returns>Placed order information.</returns>
        [Post("/accounts/{accountId}/buys")]
        Task<Response<CoinbaseSchema.PlaceOrderResponse>> PlaceBuyOrderAsync([Path] string accountId, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        /// <summary>
        /// Sells a user-defined amount of bitcoin, bitcoin cash, litecoin or ethereum.
        /// </summary>
        /// <param name="accountId">The account id of user.</param>
        /// <param name="body">Post body.</param>
        /// <returns>Placed order information.</returns>
        [Post("/accounts/{accountId}/sells")]
        Task<Response<CoinbaseSchema.PlaceOrderResponse>> PlaceSellOrderAsync([Path] string accountId, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        /// <summary>
        /// Lists sells for an account.
        /// </summary>
        /// <param name="accountId">The account which sells should be listed.</param>
        /// <returns>List of sell orders.</returns>
        [Get("/accounts/{accountId}/sells")]
        Task<Response<CoinbaseSchema.OrdersListResponse>> ListSellsAsync([Path] string accountId);

        /// <summary>
        /// Lists buys for an account.
        /// </summary>
        /// <param name="accountId">The account which buy should be listed.</param>
        /// <returns>List of buy orders.</returns>
        [Get("/accounts/{accountId}/buys")]
        Task<Response<CoinbaseSchema.OrdersListResponse>> ListBuysAsync([Path] string accountId);

        /// <summary>
        /// Show an individual buy order.
        /// </summary>
        /// <param name="accountId">The account which was used to place an order.</param>
        /// <param name="buyId">The id of buy order.</param>
        /// <returns>Information about buy order.</returns>
        [Get("/accounts/{accountId}/buys/{buyId}")]
        Task<Response<CoinbaseSchema.ShowOrderResponse>> ShowBuyOrder([Path] string accountId, [Path] string buyId);

        /// <summary>
        /// Show an individual sell order.
        /// </summary>
        /// <param name="accountId">The account which was used to place an order.</param>
        /// <param name="buyId">The id of sell order.</param>
        /// <returns>Information about sell order.</returns>
        [Get("/accounts/{accountId}/sells/{buyId}")]
        Task<Response<CoinbaseSchema.ShowOrderResponse>> ShowSellOrder([Path] string accountId, [Path] string buyId);

        /// <summary>
        /// Lists current user’s payment methods.
        /// </summary>
        /// <returns>List of payment methods.</returns>
        [Get("/payment-methods")]
        Task<Response<CoinbaseSchema.PaymentMethods>> GetPaymentMethodsAsync();

        /// <summary>
        /// Gets the current market price for bitcoin. This is usually somewhere in between the buy and sell price.
        /// </summary>
        /// <param name="currencyPair">Currency pair which current price should be returned.</param>
        /// <returns>Current price on specified market.</returns>
        [Get("/prices/{currencyPair}/spot")]
        Task<CoinbaseSchema.SpotPriceResponse> GetLatestPriceAsync([Path] string currencyPair);

        /// <summary>
        /// Gets the API server time. Used to test connection with server.
        /// </summary>
        /// <returns>Server time.</returns>
        [Get("/time")]
        Task<CoinbaseSchema.TimeResponse> GetCurrentServerTimeAsync();
    }
}