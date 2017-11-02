using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Cex
{
    internal interface ICexApi
    {
        [Get("/tickers/USD/EUR/RUB/BTC")]
        Task<CexSchema.TickersResponse> GetTickers();
    }
}
