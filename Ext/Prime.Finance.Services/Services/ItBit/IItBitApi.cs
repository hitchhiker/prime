using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.ItBit
{
    internal interface IItBitApi
    {
        [Get("/markets/{pairTicker}/ticker")]
        Task<ItBitSchema.TickerResponse> GetTickerAsync([Path] string pairTicker);
    }
}
