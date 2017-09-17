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
        Task<String> GetUserDepositAddress([Path]String currency);

        [Get("api/v1/user/wallet?currency={currency}")]
        Task<BitMexSchema.WalletInfo> GetUserWalletInfo([Path]String currency);

        [Get("api/v1/user")]
        Task<BitMexSchema.UserInfo> GetUserInfo();

        [Get("api/v1/instrument/active")]
        Task<BitMexSchema.InstrumentsActiveResponse> GetInstrumentsActive();
    }
}
