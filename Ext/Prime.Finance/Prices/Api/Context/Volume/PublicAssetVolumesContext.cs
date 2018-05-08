using System.Collections.Generic;
using System.Linq;
using Prime.Core;

namespace Prime.Finance
{
    public class PublicAssetVolumesContext : PublicVolumesContext
    {
        public readonly List<Asset> Assets;
        public readonly Asset QuoteAsset;

        public PublicAssetVolumesContext(List<Asset> assets, Asset quoteAsset, ILogger logger = null) : base(assets.Select(x => new AssetPair(x, quoteAsset)).ToList(), logger)
        {
            Assets = assets;
            QuoteAsset = quoteAsset;
        }
    }
}