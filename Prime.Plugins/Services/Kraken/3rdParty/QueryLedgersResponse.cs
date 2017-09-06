using System.Collections.Generic;

namespace KrakenApi
{
    public class QueryLedgersResponse : ResponseBase
    {
        public Dictionary<string, LedgerInfo> Result;
    }
}