using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bittrex
{
    internal interface IBittrexApi
    {
        [Get("/account/getbalances")]
        Task<BittrexSchema.BalancesResponse> GetAllBalances();
    }
}
