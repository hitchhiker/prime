namespace Prime.Finance.Services.Services.BitMarket
{
    internal class BitMarketSchema
    {
        internal class TickerResponse
        {
            public decimal ask;
            public decimal bid;
            public decimal last;
            public decimal low;
            public decimal high;
            public decimal vwap;
            public decimal volume;
        }
    }
}
