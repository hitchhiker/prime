using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Core
{
    public class PublicPricesContext : NetworkProviderContext
    {
        public readonly Asset Asset;

        public readonly List<Asset> Assets;

        public PublicPricesContext(Asset asset, List<Asset> assets, ILogger logger = null) : base(logger)
        {
            Asset = asset;
            Assets = assets;
        }
    }
}