using System.Collections.Generic;

namespace Prime.Finance.Services.Services.BitcoinIndonesia
{
    internal class BitcoinIndonesiaSchema
    {
        internal class TickerResponse
        {
            public Dictionary<string, decimal> ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal high;
            public decimal low;
            public decimal vol_base;
            public decimal vol_quote;
            public decimal last;
            public decimal buy;
            public decimal sell;
            public long server_time;
        }
    }
}
