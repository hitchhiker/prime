using System.Collections.Generic;

namespace KrakenApi
{
    public class GetTradesHistoryResult
    {
        public Dictionary<string, TradeInfo> Trades;
        public int Count;
    }
}