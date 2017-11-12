using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitStamp
{
    internal interface IBitStampApi
    {
        [Get(BitStampProvider.BitStampApiVersion + "/ticker/{currency_pair}/")]
        Task<BitStampSchema.TickerResponse> GetTickerAsync([Path("currency_pair")] string currencyPair);

        [Post(BitStampProvider.BitStampApiVersion + "/balance/")]
        Task<BitStampSchema.AccountBalancesResponse> GetAccountBalancesAsync();

        [Post("{currency}/")]
        Task<string> GetDepositAddressAsync([Path(UrlEncode = false)] string currency);

        [Post(BitStampProvider.BitStampApiVersion + "/order_book/{currencyPair}/")]
        Task<BitStampSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
