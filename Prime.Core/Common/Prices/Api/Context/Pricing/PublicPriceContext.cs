﻿using Prime.Core;
using System.Collections.Generic;

namespace Prime.Core
{
    public class PublicPriceContext : PublicAssetPricesContext
    {
        public PublicPriceContext(AssetPair pair, ILogger logger = null) : base(new List<Asset>(){pair.Asset1}, pair.Asset2, logger)
        {
        }

        public override bool UseBulkContext => false;
    }
}