using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinone
{
    internal interface ICoinoneApi
    {
        /// <summary>
        /// Gets all tickers.
        /// See http://doc.coinone.co.kr/#api-Public-Ticker.
        /// </summary>
        /// <returns>Tickers for all supported currencies.</returns>
        [Get("/ticker/?currency=all")]
        Task<CoinoneSchema.TickersResponse> GetTickersAsync();

        /// <summary>
        /// Gets ticker for specified currency.
        /// See http://doc.coinone.co.kr/#api-Public-Ticker.
        /// </summary>
        /// <param name="currency">Currency which ticker is to be returned.</param>
        /// <returns>Ticker for specified currency</returns>
        [Get("/ticker/?currency={currency}")]
        Task<CoinoneSchema.TickerResponse> GetTickerAsync([Path] string currency);
    }
}
