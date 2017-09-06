using System.Collections.Generic;

namespace KrakenApi
{
    public class QueryOrdersResponse : ResponseBase
    {
        public Dictionary<string, OrderInfo> Result;
    }
}