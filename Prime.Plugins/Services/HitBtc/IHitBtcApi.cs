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
        [Get("/1/public/symbols")]
        Task<HitBtcSchema.SymbolsResponse> GetSymbols();

        /// <summary>
        /// Gets deposit address for specified currency. If does not exist, it will be created.
        /// See https://hitbtc.com/api#getaddress.
        /// </summary>
        /// <param name="currency">Currency code which deposit address is to be returned.</param>
        /// <returns>Deposit address of specified currency.</returns>
        [Get("/1/payment/address/{currency}")]
        Task<HitBtcSchema.DepositAddressResponse> GetDepositAddress([Path] string currency);

        /// <summary>
        /// Gets payment balances.
        /// See https://hitbtc.com/api#paymentbalance.
        /// </summary>
        /// <returns>Multi-currency balance of the main account.</returns>
        [Get("/1/payment/balance")]
        Task<HitBtcSchema.BalancesResponse> GetBalances();

        /// <summary>
        /// Gets tickers for all currencies.
        /// See https://hitbtc.com/api#alltickers.
        /// </summary>
        /// <returns>Associative array of pair code and ticker data</returns>
        [Get("/1/public/ticker")]
        Task<HitBtcSchema.TickersResponse> GetAllTickers();

        /// <summary>
        /// Get ticker for specified currency pair.
        /// See https://hitbtc.com/api#ticker.
        /// </summary>
        /// <param name="pairCode">Currency which ticker is to be returned.</param>
        /// <returns>Ticker data.</returns>
        [Get("/1/public/{pairCode}/ticker")]
        Task<HitBtcSchema.TickerResponse> GetTicker([Path] string pairCode);
    }
}