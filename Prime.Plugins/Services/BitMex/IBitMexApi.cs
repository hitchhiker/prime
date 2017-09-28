using System;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    internal interface IBitMexApi
    {
        [Get("/user/depositAddress?currency={currency}")]
        // TODO: Change String to custom class in BitMexProvider.
        Task<String> GetUserDepositAddressAsync([Path]String currency);

        [Get("/user/wallet?currency={currency}")]
        Task<BitMexSchema.WalletInfo> GetUserWalletInfoAsync([Path]String currency);

        [Get("/user")]
        Task<BitMexSchema.UserInfo> GetUserInfoAsync();

        [Get("/trade/bucketed?binSize={binSize}&partial=false&count=500&symbol={currencySymbol}&reverse=true&startTime={startTime}&endTime={endTime}")]
        Task<BitMexSchema.BucketedTradeEntriesResponse> GetTradeHistory([Path] string currencySymbol, [Path] string binSize, [Path] DateTime startTime, [Path] DateTime endTime);

        [Get("/instrument/active")]
        Task<BitMexSchema.InstrumentsActiveResponse> GetInstrumentsActive();

        [Get("/instrument?symbol={currencySymbol}&columns=[\"lastPrice\",\"timestamp\",\"symbol\"]&reverse=true")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPriceAsync([Path]String currencySymbol);

        [Get("/instrument?columns=[\"lastPrice\",\"timestamp\",\"symbol\",\"underlying\",\"quoteCurrency\"]&reverse=true&count=500")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPricesAsync();
    }
}
