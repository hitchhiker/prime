using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Bitlish
{
    [AllowAnyStatusCode]
    internal interface IBitlishApi
    {
        [Get("/tickers")]
        Task<BitlishSchema.AllTickersResponse> GetTickersAsync();

        [Get("/trades_depth?pair_id={currencyPair}")]
        Task<BitlishSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/signin")]
        Task<Response<BitlishSchema.AuthenticationResponse>> AuthenticateUserAsync();

        [Get("/create_trade?pair_id={currencyPair}&dir={side}&amount={amount}&price={price}&authentication_token={authenticationToken}")]
        Task<Response<BitlishSchema.OrderResponse>> PlaceNewOrderAsync([Path(UrlEncode = false)] string authenticationToken, [Path] string currencyPair, [Path]string side, [Path]decimal amount, [Path]decimal price);

        [Get("/trade_details?id={orderId}&authentication_token={authenticationToken}")]
        Task<Response<BitlishSchema.OrderResponse>> QueryOrderAsync([Path(UrlEncode = false)] string authenticationToken, [Path] string orderId);

        [Get("/create_template_wallet?payment_method={paymentMethod}&account={account}&authentication_token={authenticationToken}")]
        Task<Response<BitlishSchema.TemplateWalletResponse>> CreateTemplateWalletAsync([Path(UrlEncode = false)] string authenticationToken, [Path] string paymentMethod, [Path] string account);

        [Get("/withdraw_by_id?wallet_id={walletId}&amount={amount}&currency={currency}&authentication_token={authenticationToken}")]
        Task<Response<BitlishSchema.WithdrawalResponse>> PlaceWithdrawalAsync([Path(UrlEncode = false)] string authenticationToken, [Path] string walletId, [Path] decimal amount, [Path] string currency);
    }
}
