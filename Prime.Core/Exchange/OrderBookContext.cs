using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    public class OrderBookContext : OrderBookLiveContext
    {
        public OrderBookContext(AssetPair assetPair, int depth, ILogger logger = null) : base(assetPair, logger)
        {
            Depth = depth;
        }

        public int Depth { get; set; }
    }
}
