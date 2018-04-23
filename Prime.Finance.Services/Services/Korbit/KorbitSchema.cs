namespace Prime.Finance.Services.Services.Korbit
{
    internal class KorbitSchema
    {
        internal class TickerResponse
        {
            public long timestamp;
            public decimal last;
        }

        internal class DetailedTickerResponse : TickerResponse
        {
            public decimal bid;
            public decimal ask;
            public decimal low;
            public decimal high;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
