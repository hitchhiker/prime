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
        Task<BitMexSchema.BucketedTradeEntriesResponse> GetTradeHistory([Path] string currencySymbol, [Path] string binSize, [Path(Format = "yyyy.MM.dd")] DateTime startTime, [Path(Format = "yyyy.MM.dd")] DateTime endTime);

        [Get("/instrument/active")]
        Task<BitMexSchema.InstrumentsActiveResponse> GetInstrumentsActive();

        [Get("/instrument?symbol={currencySymbol}&columns=[\"lastPrice\",\"timestamp\",\"symbol\",\"volume24h\"]&reverse=true")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPriceAsync([Path]String currencySymbol);

        [Get("/instrument?columns=[\"lastPrice\",\"timestamp\",\"symbol\",\"underlying\",\"quoteCurrency\"]&reverse=true&count=500")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPricesAsync();

        [Get("/orderBook/L2?symbol={currencyPair}&depth={depth}")]
        Task<BitMexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int depth);

        [Get("/user/walletHistory?currency={currency}")]
        Task<BitMexSchema.WalletHistoryResponse> GetWalletHistory([Path] string currency);

        [Post("/user/requestWithdrawal")]
        Task<BitMexSchema.WithdrawalRequestResponse> RequestWithdrawal([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/user/cancelWithdrawal")]
        Task<BitMexSchema.WithdrawalCancelationResponse> CancelWithdrawal([Body(BodySerializationMethod.UrlEncoded)]Dictionary<string, object> body);

        [Post("/user/confirmWithdrawal")]
        Task<BitMexSchema.WithdrawalConfirmationResponse> ConfirmWithdrawal([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
