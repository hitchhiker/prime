using System.Collections.Generic;

namespace KrakenApi
{
    public class GetOrderBookResponse : ResponseBase
    {
        public Dictionary<string, OrderBook> Result;
    }
}