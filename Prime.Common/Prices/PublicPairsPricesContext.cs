using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicPairsPricesContext : NetworkProviderContext
    {
        public readonly List<AssetPair> Pairs;

        public PublicPairsPricesContext(List<AssetPair> pairs, ILogger logger = null) : base(logger)
        {
            Pairs = pairs;
        }
    }
}