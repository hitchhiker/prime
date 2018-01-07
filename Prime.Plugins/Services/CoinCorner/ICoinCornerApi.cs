using RestEase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.CoinCorner
{
    internal interface ICoinCornerApi
    {
        [Get("/Ticker?Coin={Coin}&Currency={Currency}")]
        Task<CoinCornerSchema.TickerResponse> GetTickerAsync([Path] string Coin, [Path] string Currency);

        [Get("/OrderBook?Coin={Coin}")]
        Task<CoinCornerSchema.OrderBookResponse> GetOrderBookAsync([Path] string Coin);
    }
}
