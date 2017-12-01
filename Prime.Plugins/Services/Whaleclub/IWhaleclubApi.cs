using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Whaleclub
{
    internal interface IWhaleclubApi
    {
        [Get("/markets")]
        Task<WhaleclubSchema.MarketsResponse> GetMarkets();
    }
}