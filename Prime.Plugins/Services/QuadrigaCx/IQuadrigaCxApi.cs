using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.QuadrigaCx
{
    internal interface IQuadrigaCxApi
    {
        [Get("/ticker?book={currencyPair}")]
        Task<QuadrigaCxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
