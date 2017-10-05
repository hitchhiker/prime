using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    public class OrderBookContext : NetworkProviderContext
    {
        public OrderBookContext(AssetPair pair, int depth, ILogger logger = null) : base(logger)
        {
            Pair = pair;
            Depth = depth;
        }

        public AssetPair Pair { get; set; }
        public int Depth { get; set; }
    }
}
