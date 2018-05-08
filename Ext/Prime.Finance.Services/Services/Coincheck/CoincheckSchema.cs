namespace Prime.Finance.Services.Services.Coincheck
{
    internal class CoincheckSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public long timestamp;
        }

        internal class OrderBookResponse
        {
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
