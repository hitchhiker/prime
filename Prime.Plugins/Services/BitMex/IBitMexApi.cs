using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    [AllowAnyStatusCode]
    internal interface IBitMexApi
    {
        [Get("/user/depositAddress?currency={currency}")]
        Task<String> GetUserDepositAddressAsync([Path]String currency);

        [Get("/user/wallet?currency={currency}")]
        Task<Response<BitMexSchema.WalletInfoResponse>> GetUserWalletInfoAsync([Path]String currency);

        [Get("/user")]
        Task<Response<BitMexSchema.UserInfoResponse>> GetUserInfoAsync();

        [Get("/order")]
        Task<Response<BitMexSchema.OrdersResponse>> GetOrdersAsync(
            [Query] string symbol = null, 
            [Query] string filter = null,
            [Query] string columns = null, 
            [Query] double? count = null, 
            [Query] double? start = null,
            [Query] bool? reverse = null, 
            [Query] DateTime? startTime = null, 
            [Query] DateTime? endTime = null);

        [Post("/order")]
        Task<Response<BitMexSchema.NewOrderResponse>> CreateNewLimitOrderAsync([Query] string ordType, [Query] string side, [Query] string symbol, [Query] decimal orderQty, [Query] decimal price);

        [Get("/trade/bucketed?binSize={binSize}&partial=false&count=500&symbol={currencySymbol}&reverse=true&startTime={startTime}&endTime={endTime}")]
        Task<Response<BitMexSchema.BucketedTradeEntriesResponse>> GetTradeHistoryAsync([Path] string currencySymbol, [Path] string binSize, [Path(Format = "yyyy.MM.dd")] DateTime startTime, [Path(Format = "yyyy.MM.dd")] DateTime endTime);

        [Get("/instrument/active")]
        Task<Response<BitMexSchema.InstrumentsActiveResponse>> GetInstrumentsActiveAsync();

        [Get("/instrument?columns=[\"underlying\",\"quoteCurrency\",\"lastPrice\",\"highPrice\",\"lowPrice\",\"bidPrice\",\"askPrice\",\"timestamp\",\"symbol\",\"volume24h\"]&reverse=true&count=500&filter=%7B\"state\": \"Open\", \"typ\": \"FFWCSX\"%7D")]
        Task<Response<BitMexSchema.InstrumentLatestPricesResponse>> GetLatestPricesAsync([Query("symbol")] string pairCode = null);

        [Get("/orderBook/L2?symbol={currencyPair}&depth={depth}")]
        Task<Response<BitMexSchema.OrderBookResponse>> GetOrderBookAsync([Path] string currencyPair, [Path] int depth);

        [Get("/user/walletHistory?currency={currency}")]
        Task<Response<BitMexSchema.WalletHistoryResponse>> GetWalletHistoryAsync([Path] string currency);

        [Post("/user/requestWithdrawal")]
        Task<Response<BitMexSchema.WithdrawalRequestResponse>> RequestWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/user/cancelWithdrawal")]
        Task<Response<BitMexSchema.WithdrawalCancelationResponse>> CancelWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)]Dictionary<string, object> body);

        [Post("/user/confirmWithdrawal")]
        Task<Response<BitMexSchema.WithdrawalConfirmationResponse>> ConfirmWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/chat/connected")]
        Task<Response<BitMexSchema.ConnectedUsersResponse>> GetConnectedUsersAsync();
    }
}
