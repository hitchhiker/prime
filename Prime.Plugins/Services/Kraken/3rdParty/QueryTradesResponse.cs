using System.Collections.Generic;

namespace KrakenApi
{
    public class QueryTradesResponse : ResponseBase
    {
        public Dictionary<string, TradeInfo> Result;
    }
}