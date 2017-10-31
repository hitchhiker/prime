using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Coinone
{
    internal class CoinoneSchema
    {
        internal class BaseResponse
        {
            public string result;
            public string errorCode;
        }

        internal class TickerResponse : TickerEntryResponse { }

        internal class TickersResponse : Dictionary<string, object> { }

        internal class TickerEntryResponse : BaseResponse
        {
            public decimal volume;
            public decimal last;
            public decimal yesterday_last;
            public decimal yesterday_low;
            public decimal high;
            public string currency;
            public decimal low;
            public decimal yesterday_first;
            public decimal yesterday_volume;
            public decimal yesterday_high;
            public decimal first;
        }
    }
}
