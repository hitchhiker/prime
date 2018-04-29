using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
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
