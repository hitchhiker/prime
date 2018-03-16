using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
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
