using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitStamp
{
    internal interface IBitStampApi
    {
        [Get("ticker/{currency_pair}/")]
        Task<BitStampSchema.TickerResponse> GetTicker([Path("currency_pair")] string currencyPair);
    }
}
