using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BlinkTrade
{
    internal interface IBlinkTradeApi
    {
        [Get("/{fiat}/ticker?crypto_currency={crypto}")]
        Task<BlinkTradeSchema.TickerResponse> GetTickerAsync([Path] string fiat, [Path] string crypto);

        [Get("/{fiat}/orderbook?crypto_currency={crypto}")]
        Task<BlinkTradeSchema.OrderBookResponse> GetOrderBookAsync([Path] string fiat, [Path] string crypto);
    }
}
