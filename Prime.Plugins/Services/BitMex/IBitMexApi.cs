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
        [Get("api/v1/user/depositAddress?currency={currency}")]
        // TODO: Change String to custom class in BitMexProvider.
        Task<String> GetUserDepositAddressAsync([Path]String currency);

        [Get("api/v1/user/wallet?currency={currency}")]
        Task<BitMexSchema.WalletInfo> GetUserWalletInfoAsync([Path]String currency);

        [Get("api/v1/user")]
        Task<BitMexSchema.UserInfo> GetUserInfoAsync();

        [Get("api/v1/instrument/active")]
        Task<BitMexSchema.InstrumentsActiveResponse> GetInstrumentsActive();

        [Get("api/v1/instrument?symbol={currencySymbol}&columns=[\"lastPrice\",\"timestamp\",\"symbol\"]&reverse=true")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPriceAsync([Path]String currencySymbol);

        [Get("api/v1/instrument?columns=[\"lastPrice\",\"timestamp\",\"symbol\",\"underlying\",\"quoteCurrency\"]&reverse=true&count=500")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPricesAsync();
    }
}
