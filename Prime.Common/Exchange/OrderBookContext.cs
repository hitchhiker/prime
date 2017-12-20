using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Common
{
    public class OrderBookContext : NetworkProviderContext
    {
        public OrderBookContext(AssetPair assetPair, int maxRecordsCount = 1000, ILogger logger = null) : base(logger)
        {
            Pair = assetPair;
            MaxRecordsCount = maxRecordsCount;
        }

        public AssetPair Pair { get; set; }

        /// <summary>
        /// Set Int32.MaxValue to return all records from endpoint (provider won't set any limits when querying endpoint if supported or will set the maximum number that is specified in API docs). 
        /// </summary>
        public int MaxRecordsCount { get; set; }
    }
}
