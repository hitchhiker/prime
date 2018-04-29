using System.Collections.Generic;
using Prime.Core;

namespace Prime.Core
{
    public class PublicVolumeContext : PublicAssetVolumesContext
    {
        public PublicVolumeContext(AssetPair pair, ILogger logger = null) : base(new List<Asset>() { pair.Asset1 }, pair.Asset2, logger)
        {
        }

        public override bool UseBulkContext => false;
    }
}
