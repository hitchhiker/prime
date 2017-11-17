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

        public AssetPair Pair => Pairs.FirstOrDefault();

        public virtual bool UseBulkContext => true;

        public bool ForSingleMethod => !UseBulkContext;
    }
}