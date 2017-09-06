using System.Collections.Generic;

namespace KrakenApi
{
    public class GetTickerResponse : ResponseBase
    {
        public Dictionary<string, Ticker> Result;
    }
}