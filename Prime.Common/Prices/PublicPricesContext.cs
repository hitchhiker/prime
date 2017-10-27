using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicPricesContext : NetworkProviderContext
    {
        public readonly List<AssetPair> Pairs;

        public PublicPricesContext(List<AssetPair> pairs, ILogger logger = null) : base(logger)
        {
            Pairs = pairs;
        }
    }
}