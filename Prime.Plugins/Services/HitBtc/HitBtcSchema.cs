using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.HitBtc
{
    internal class HitBtcSchema
    {
        internal class SymbolsResponse: List<SymbolResponse> { }

        internal class SymbolResponse
        {
            public string id;
            public string baseCurrency;
            public string quoteCurrency;
            public decimal quantityIncrement;
            public decimal tickSize;
            public decimal takeLiquidityRate;
            public decimal provideLiquidityRate;
            public string feeCurrency;
        }

        internal class TickersResponse : List<TickerResponse> { }

        internal class TickerResponse
        {
            public decimal? ask;
            public decimal? bid;
            public decimal? last;
            public decimal? open;
            public decimal? low;
            public decimal? high;
            public decimal volume;
            public decimal volumeQuote;
            public DateTime timestamp;
            public string symbol;
        }

        internal class BalancesResponse : List<BalanceResponse> { }

        internal class BalanceResponse
        {
            public string currency;
            public decimal available;
            public decimal reserved;
        }

        internal class DepositAddressResponse
        {
            public string address;
            public string paymentId;
        }
    }
}
