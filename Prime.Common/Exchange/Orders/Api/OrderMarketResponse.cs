using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class OrderMarketResponse
    {
        public OrderMarketResponse(AssetPair market)
        {
            Market = market;
        }

        public AssetPair Market { get; }
    }
}
