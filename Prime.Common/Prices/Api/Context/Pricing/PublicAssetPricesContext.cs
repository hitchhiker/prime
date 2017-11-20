using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicAssetPricesContext : PublicPricesContext
    {
        public readonly List<Asset> Assets;
        public readonly Asset QuoteAsset;

        public PublicAssetPricesContext(List<Asset> assets, Asset quoteAsset, ILogger logger = null) : base(assets.Select(x => new AssetPair(x, quoteAsset)).ToList(), logger)
        {
            Assets = assets;
            QuoteAsset = quoteAsset;
        }
    }
}