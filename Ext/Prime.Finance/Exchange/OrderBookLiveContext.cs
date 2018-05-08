﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public class OrderBookLiveContext : NetworkProviderContext
    {
        public OrderBookLiveContext(AssetPair assetPair, ILogger logger = null) : base(logger)
        {
            Pair = assetPair;
        }

        public AssetPair Pair { get; set; }
    }
}
