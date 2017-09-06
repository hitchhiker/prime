using System.Collections.Generic;

namespace KrakenApi
{
    public class GetRecentSpreadResult
    {
        public Dictionary<string, List<SpreadItem>> Spread;

        /// <summary>
        /// Id to be used as since when polling for new spread data
        /// </summary>
        public long Last;
    }
}