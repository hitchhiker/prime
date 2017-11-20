using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicPricesContext : PublicContextBase
    {
        public PublicPricesContext(IList<AssetPair> pairs, ILogger logger = null) : base(pairs, logger) { }

        public bool RequestVolume { get; set; }

        public bool RequestStatistics { get; set; }
    }
}