using System.Collections.Generic;
using System.Linq;
using Prime.Common;

namespace Prime.Common
{
    public class PublicPricesContext : PublicContextBase
    {
        public PublicPricesContext(ILogger logger = null) : base(logger) { }

        public PublicPricesContext(IList<AssetPair> pairs, ILogger logger = null) : base(pairs, logger) { }

        public bool RequestVolume { get; set; }

        public bool RequestStatistics { get; set; }
    }
}