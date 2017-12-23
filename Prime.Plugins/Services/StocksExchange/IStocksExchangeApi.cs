using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.StocksExchange
{
    internal interface IStocksExchangeApi
    {
        [Get("/ticker")]
        Task<StocksExchangeSchema.AllTickersResponse[]> GetTickersAsync();
    }
}
