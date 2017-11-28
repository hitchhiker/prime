using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    internal interface IBitMexApi
    {
        [Get("/user/depositAddress?currency={currency}")]
        Task<String> GetUserDepositAddressAsync([Path]String currency);

        [Get("/user/wallet?currency={currency}")]
        Task<BitMexSchema.WalletInfoResponse> GetUserWalletInfoAsync([Path]String currency);

        [Get("/user")]
        Task<BitMexSchema.UserInfoResponse> GetUserInfoAsync();

        [Get("/trade/bucketed?binSize={binSize}&partial=false&count=500&symbol={currencySymbol}&reverse=true&startTime={startTime}&endTime={endTime}")]
        Task<BitMexSchema.BucketedTradeEntriesResponse> GetTradeHistoryAsync([Path] string currencySymbol, [Path] string binSize, [Path(Format = "yyyy.MM.dd")] DateTime startTime, [Path(Format = "yyyy.MM.dd")] DateTime endTime);

        [Get("/instrument/active")]
        Task<BitMexSchema.InstrumentsActiveResponse> GetInstrumentsActiveAsync();

        [Get("/instrument?columns=[\"underlying\",\"quoteCurrency\",\"lastPrice\",\"highPrice\",\"lowPrice\",\"bidPrice\",\"askPrice\",\"timestamp\",\"symbol\",\"volume24h\"]&reverse=true&count=500&filter=%7B\"state\": \"Open\", \"typ\": \"FFWCSX\"%7D")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPricesAsync([Query("symbol")] string pairCode = null);

        [Get("/orderBook/L2?symbol={currencyPair}&depth={depth}")]
        Task<BitMexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int depth);

        [Get("/user/walletHistory?currency={currency}")]
        Task<BitMexSchema.WalletHistoryResponse> GetWalletHistoryAsync([Path] string currency);

        [Post("/user/requestWithdrawal")]
        Task<BitMexSchema.WithdrawalRequestResponse> RequestWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/user/cancelWithdrawal")]
        Task<BitMexSchema.WithdrawalCancelationResponse> CancelWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)]Dictionary<string, object> body);

        [Post("/user/confirmWithdrawal")]
        Task<BitMexSchema.WithdrawalConfirmationResponse> ConfirmWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/chat/connected")]
        Task<BitMexSchema.ConnectedUsersResponse> GetConnectedUsersAsync();
    }
}
