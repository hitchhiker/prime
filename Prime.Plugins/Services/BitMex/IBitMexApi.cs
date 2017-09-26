using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using plugins.Services.BitMex;
using RestEase;

namespace plugins
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

        [Get("/trade/bucketed?binSize={binSize}&partial=false&symbol={currencySymbol}&count={count}&reverse=true")]
        Task<BitMexSchema.BucketedTradeEntriesResponse> GetTradeHistory([Path] string currencySymbol, [Path] string binSize, [Path] int count);

        [Get("/instrument/active")]
        Task<BitMexSchema.InstrumentsActiveResponse> GetInstrumentsActive();

        [Get("/instrument?symbol={currencySymbol}&columns=[\"lastPrice\",\"timestamp\",\"symbol\"]&reverse=true")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPriceAsync([Path]String currencySymbol);

        [Get("/instrument?columns=[\"lastPrice\",\"timestamp\",\"symbol\",\"underlying\",\"quoteCurrency\"]&reverse=true&count=500")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPricesAsync();
    }
}
