namespace Prime.Finance.Services.Services.BtcMarkets
{
    internal class BtcMarketsSchema
    {
        internal class TickerResponse
        {
            public decimal lastPrice;
            public decimal volume24h;
            public decimal bestBid;
            public decimal bestAsk;
            public long timestamp;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public string currency;
            public string instrument;
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
