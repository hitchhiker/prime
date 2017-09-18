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

        /// <summary>
        /// Select only lastPrice, timestamp and symbol columns.
        /// </summary>
        /// <param name="currencySymbol"></param>
        /// <returns></returns>
        [Get("api/v1/instrument?symbol={currencySymbol}&columns=%5B%22lastPrice%22%2C%20%22timestamp%22%2C%20%22symbol%22%5D&reverse=true")]
        Task<BitMexSchema.InstrumentLatestPricesResponse> GetLatestPriceAsync([Path]String currencySymbol);
    }
}
