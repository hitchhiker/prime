using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicAssetPricesContext : NetworkProviderContext
    {
        public readonly Asset QuoteAsset;

        public readonly List<Asset> Assets;

        public PublicAssetPricesContext(List<Asset> assets, Asset quoteAsset, ILogger logger = null) : base(logger)
        {
            QuoteAsset = quoteAsset;
            Assets = assets;
        }
    }
}