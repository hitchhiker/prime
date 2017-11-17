using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicPricesContext : NetworkProviderContext
    {
        public readonly IList<AssetPair> Pairs;

        public PublicPricesContext(IList<AssetPair> pairs, ILogger logger = null) : base(logger)
        {
            Pairs = pairs;
        }

        public bool RequestVolume { get; set; }

        public bool RequestStatistics { get; set; }

        public AssetPair Pair => Pairs.FirstOrDefault();

        public virtual bool UseBulkContext => true;

        public bool IsMultiple => Pairs.Count > 1;

        public bool ForSingleMethod => !UseBulkContext;
    }
}