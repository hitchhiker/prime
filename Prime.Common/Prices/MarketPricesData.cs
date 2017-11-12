using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Prime.Utility;

namespace Prime.Common.Prices.Latest
{
    public class MarketPricesData : ModelBase
    {
        private MarketPricesData() { }

        public MarketPricesData(Network network, List<MarketPrice> prices)
        {
            Network = network;
            Id = GetHash(Network);
            Prices = prices;
        }

        public static ObjectId GetHash(Network network)
        {
            return ("latestpricenetworkdata:" + network.Id).GetObjectIdHashCode();
        }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public List<MarketPrice> Prices { get; private set; }
    }
}
