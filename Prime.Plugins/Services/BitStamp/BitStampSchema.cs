using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins.Services.BitStamp
{
    internal class BitStampSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public long timestamp;
            public decimal open;
        }

        internal class AccountBalancesResponse
        {
            public decimal usd_balance;
            public decimal btc_balance;
            public decimal eur_balance;
            public decimal xrp_balance;
            public decimal usd_reserved;
            public decimal btc_reserved;
            public decimal eur_reserved;
            public decimal xrp_reserved;
            public decimal usd_available;
            public decimal btc_available;
            public decimal eur_available;
            public decimal xrp_available;

            public decimal btcusd_fee;
            public decimal btceur_fee;
            public decimal eurusd_fee;
            public decimal xrpusd_fee;
            public decimal xrpeur_fee;
            public decimal xrpbtc_fee;

            public decimal fee;
        }
    }
}
