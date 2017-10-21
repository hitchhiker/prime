using System.Collections.Generic;
using Prime.Common.Messages;

namespace Prime.Common
{
    public class AssetAllResponseMessage : RequestorTokenMessageBase
    {
        public readonly List<Asset> Assets;

        public AssetAllResponseMessage(List<Asset> assets, string requesterToken) : base(requesterToken)
        {
            Assets = assets;
        }
    }
}