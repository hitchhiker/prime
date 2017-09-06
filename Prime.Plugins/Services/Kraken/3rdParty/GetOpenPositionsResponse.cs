using System.Collections.Generic;

namespace KrakenApi
{
    public class GetOpenPositionsResponse : ResponseBase
    {
        public Dictionary<string, PositionInfo> Result;
    }
}