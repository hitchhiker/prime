using System.Collections.Generic;
using Prime.Core.Messages;

namespace Prime.Core
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