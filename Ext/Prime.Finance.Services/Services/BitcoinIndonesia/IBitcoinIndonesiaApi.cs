using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.BitcoinIndonesia
{
    internal interface IBitcoinIndonesiaApi
    {
        [Get("/{currencyPair}/ticker")]
        Task<BitcoinIndonesiaSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);
    }
}
