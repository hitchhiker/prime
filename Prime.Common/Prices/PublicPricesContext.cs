using System.Collections.Generic;
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
    }
}