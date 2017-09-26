using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using plugins.Services.BitStamp;
using RestEase;

namespace plugins
{
    internal interface IBitStampApi
    {
        [Get(BitStampProvider.BitStampApiVersion + "/ticker/{currency_pair}/")]
        Task<BitStampSchema.TickerResponse> GetTicker([Path("currency_pair")] string currencyPair);

        [Get(BitStampProvider.BitStampApiVersion + "/balance/")]
        Task<BitStampSchema.AccountBalancesResponse> GetAccountBalances();

        [Get("{currency}/")]
        Task<string> GetDepositAddress([Path] string currency);
    }
}
