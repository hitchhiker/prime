﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Binance
{
    [AllowAnyStatusCode]
    internal interface IBinanceApi
    {
        /// <summary>
        /// For more details see https://www.binance.com/restapipub.html#user-content-market-data-endpoints.
        /// </summary>
        /// <returns></returns>
        [Get("/v1/ticker/allPrices")]
        Task<Response<BinanceSchema.LatestPricesResponse>> GetSymbolPriceTickerAsync();

        /// <summary>
        /// Maximum number of order book records is 100.
        /// </summary>
        /// <param name="symbol">Currency pair code.</param>
        /// <param name="limit">The maximum number or records to return.</param>
        /// <returns>List of order book entries.</returns>
        [Get("/v1/depth?symbol={symbol}&limit={limit}")]
        Task<Response<BinanceSchema.OrderBookResponse>> GetOrderBookAsync([Path] string symbol, [Path] int limit = 100);

        /// <summary>
        /// Gets user account information.
        /// For more details see https://www.binance.com/restapipub.html#user-content-account-endpoints.
        /// </summary>
        /// <returns>Information about user account.</returns>
        [Get("/v3/account")]
        Task<Response<BinanceSchema.UserInformationResponse>> GetAccountInformationAsync();
        
        [Post("/v3/order")]
        Task<Response<BinanceSchema.NewOrderResponse>> TestNewOrderAsync([Query] string symbol, [Query] string side, [Query] string type, [Query] string timeInForce, [Query] decimal quantity, [Query] decimal price, [Query] string newClientOrderId = null, [Query] decimal? stopPrice = null, [Query] decimal? icebergQty = null, [Query] long? recvWindow = null);

        /// <summary>
        /// Gets OHLC data.
        /// </summary>
        /// <param name="currency">Currency pair code being queried.</param>
        /// <param name="interval">Time interval.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="limit">Limits the number of records to be returned.</param>
        /// <returns>Array of decimals that represent OHLC record.</returns>
        [Get("/v1/klines?symbol={currency}&interval={interval}")]
        Task<Response<BinanceSchema.CandlestickResponse>> GetCandlestickBarsAsync([Path] string currency, [Path] string interval, [Query] long? startTime = null, [Query] long? endTime = null, [Query] int? limit = null);

        /// <summary>
        /// Gets 24-hr ticker.
        /// </summary>
        /// <param name="currency">Currency which ticker is to be returned.</param>
        /// <returns>24 hour ticker.</returns>
        [Get("/v1/ticker/24hr?symbol={currency}")]
        Task<Response<BinanceSchema.Ticker24HrResponse>> Get24HrTickerAsync([Path] string currency);

        /// <summary>
        /// Checks server availability.
        /// </summary>
        /// <returns></returns>
        [Get("/v1/ping")]
        Task<Response<object>> PingAsync();
    }
}
