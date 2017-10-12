using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    internal enum OrderBookDepthLevel
    {
        Latest = 1,
        Top50Aggregated = 2,
        FullNonAggregated = 3
    }
}
