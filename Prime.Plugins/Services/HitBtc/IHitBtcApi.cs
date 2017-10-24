using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.HitBtc
{
    internal interface IHitBtcApi
    {
        /// <summary>
        /// Gets symbols with their characteristics supported by exchange.
        /// https://hitbtc.com/api#symbols
        /// </summary>
        /// <returns>List of currency symbols with their characteristics.</returns>
        [Get("/1/public/symbols")]
        Task<HitBtcSchema.SymbolsResponse> GetSymbols();

        /// <summary>
        /// Gets deposit address for specified currency. If does not exist, it will be created.
        /// https://hitbtc.com/api#getaddress
        /// </summary>
        /// <param name="currency">Currency code which deposit address is to be returned.</param>
        /// <returns>Deposit address of specified currency.</returns>
        [Get("/1/payment/address/{currency}")]
        Task<HitBtcSchema.DepositAddressResponse> GetDepositAddress([Path] string currency);

        /// <summary>
        /// Gets payment balances.
        /// https://hitbtc.com/api#paymentbalance
        /// </summary>
        /// <returns>Multi-currency balance of the main account.</returns>
        [Get("/1/payment/balance")]
        Task<HitBtcSchema.BalancesResponse> GetBalances();
    }
}