using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Core
{
    public class PublicPricesContext : NetworkProviderContext
    {
        public readonly Asset BaseAsset;

        public readonly List<Asset> Assets;

        public PublicPricesContext(Asset baseAsset, List<Asset> assets, ILogger logger = null) : base(logger)
        {
            BaseAsset = baseAsset;
            Assets = assets;
        }
    }
}