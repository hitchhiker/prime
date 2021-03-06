﻿using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance
{
    public class AssetNetworkResponseMessage
    {
        public readonly Network Network;
        public readonly IReadOnlyList<Asset> Assets;

        public AssetNetworkResponseMessage(Network network, UniqueList<Asset> assets)
        {
            Network = network;
            Assets = assets;
        }
    }
}