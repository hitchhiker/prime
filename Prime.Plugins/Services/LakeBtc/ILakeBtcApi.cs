using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.LakeBtc
{
    internal interface ILakeBtcApi
    {
        [Get("/ticker")]
        Task<Dictionary<string,LakeBtcSchema.TickerResponse>> GetTickersAsync();
    }
}
