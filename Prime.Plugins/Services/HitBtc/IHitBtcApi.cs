using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.HitBtc
{
    internal interface IHitBtcApi
    {
        /// <summary>
        /// Gets symbols with their characteristics supported by exchange.
        /// See https://hitbtc.com/api#symbols.
        /// </summary>
        /// <returns>List of currency symbols with their characteristics.</returns>
        [Get("/public/symbol")]
        Task<HitBtcSchema.SymbolsResponse> GetSymbolsAsync();

        /// <summary>
        /// Gets deposit address for specified currency. If does not exist, it will be created.
        /// See https://hitbtc.com/api#getaddress.
        /// </summary>
        /// <param name="currency">Currency code which deposit address is to be returned.</param>
        /// <returns>Deposit address of specified currency.</returns>
        [Get("/account/crypto/address/{currency}")]
        Task<HitBtcSchema.DepositAddressResponse> GetDepositAddressAsync([Path] string currency);

        /// <summary>
        /// Gets payment balances.
        /// See https://hitbtc.com/api#paymentbalance.
        /// </summary>
        /// <returns>Multi-currency balance of the main account.</returns>
        [Get("/account/balance")]
        Task<HitBtcSchema.BalancesResponse> GetBalancesAsync();

        /// <summary>
        /// Gets tickers for all currencies.
        /// See https://hitbtc.com/api#alltickers.
        /// </summary>
        /// <returns>Associative array of pair code and ticker data</returns>
        [Get("/public/ticker")]
        Task<HitBtcSchema.TickersResponse> GetAllTickersAsync();

        /// <summary>
        /// Get ticker for specified currency pair.
        /// See https://hitbtc.com/api#ticker.
        /// </summary>
        /// <param name="pairCode">Currency which ticker is to be returned.</param>
        /// <returns>Ticker data.</returns>
        [Get("/public/ticker/{pairCode}")]
        Task<HitBtcSchema.TickerResponse> GetTickerAsync([Path] string pairCode);
    }
}