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
        public OrderBookContext(AssetPair assetPair, int? maxRecordsCount = null, ILogger logger = null) : base(logger)
        {
            Pair = assetPair;
            MaxRecordsCount = maxRecordsCount;
        }

        public AssetPair Pair { get; set; }

        public int? MaxRecordsCount { get; set; }
    }
}
