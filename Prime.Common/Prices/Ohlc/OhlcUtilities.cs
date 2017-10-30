using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class OhlcUtilities
    {
        public static ObjectId GetHash(AssetPair pair, TimeResolution market, Network network)
        {
            return $"prime:{pair.Asset1.ShortCode}:{pair.Asset2.ShortCode}:{(int)market}:{network.Id}".GetObjectIdHashCode(true, true);
        }
    }
}
