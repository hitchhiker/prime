using System;
using LiteDB;
using Prime.Utility.Misc;

namespace Prime.Common.Exchange.Rates
{
    public class ExchangeRateSubscribe : RenewableSubscribeMessageBase
    {
        public readonly AssetPair Pair;
        public readonly Network Network;
        public static TimeSpan DefaultSpan = TimeSpan.FromMinutes(1);

        public ExchangeRateSubscribe(ObjectId id) : base(id) { }

        public ExchangeRateSubscribe(ObjectId id, AssetPair pair, SubscriptionType type) : base(id, type)
        {
            Pair = pair;
        }

        public ExchangeRateSubscribe(ObjectId id, AssetPair pair, Network network, SubscriptionType type) : this(id, pair, type)
        {
            Network = network;
        }

        public override bool IsEquivalent(SubscribeMessageBase smb)
        {
            if (!(smb is ExchangeRateSubscribe o))
                return false;

            return Equals(o.Pair, Pair) && Network.EqualOrBothNull(o?.Network);
        }

        public override TimeSpan ExpirationSpan => DefaultSpan;
    }
}