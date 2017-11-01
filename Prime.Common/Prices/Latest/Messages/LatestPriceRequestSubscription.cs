using System;
using LiteDB;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceRequestSubscription : RenewableSubscribeMessageBase
    {
        public readonly AssetPair Pair;
        public readonly Network Network;

        public LatestPriceRequestSubscription(ObjectId subscriberId, SubscriptionType type = SubscriptionType.KeepAlive) : base(subscriberId, type)
        {
        }

        public LatestPriceRequestSubscription(ObjectId subscriberId, AssetPair pair, Network network = null, SubscriptionType type = SubscriptionType.Subscribe) : base(subscriberId, type)
        {
            Pair = pair;
            Network = network;
        }

        public override TimeSpan ExpirationSpan => ExpirationSpanDefault;

        public static TimeSpan ExpirationSpanDefault = TimeSpan.FromMinutes(1);

        public override bool IsEquivalent(SubscribeMessageBase smb)
        {
            if (!(smb is LatestPriceRequestSubscription o))
                return false;

            return Equals(o.Pair, Pair) && (o.Network == null && Network == null || Equals(o.Network, Network));
        }
    }
}