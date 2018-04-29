using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Finance
{
    public class OrderMarketResponse : ResponseModelBase
    {
        public OrderMarketResponse(AssetPair market)
        {
            Market = market;
        }

        public AssetPair Market { get; }
    }
}
