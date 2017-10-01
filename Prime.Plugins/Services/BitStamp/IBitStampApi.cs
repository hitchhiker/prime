using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitStamp
{
    internal interface IBitStampApi
    {
        [Get(BitStampProvider.BitStampApiVersion + "/ticker/{currency_pair}/")]
        Task<BitStampSchema.TickerResponse> GetTicker([Path("currency_pair")] string currencyPair);

        [Post(BitStampProvider.BitStampApiVersion + "/balance/")]
        Task<BitStampSchema.AccountBalancesResponse> GetAccountBalances();

        [Get("{currency}/")]
        Task<string> GetDepositAddress([Path] string currency);
    }
}
