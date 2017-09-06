using System.Collections.Generic;

namespace KrakenApi
{
    public class GetBalanceResponse : ResponseBase
    {
        public Dictionary<string, decimal> Result;
    }
}