using System;
using LiteDB;

namespace Prime.Common
{
    public abstract class RenewableSubscribeMessageBase : SubscribeMessageBase
    {
        protected RenewableSubscribeMessageBase(ObjectId subscriberId, SubscriptionType type = SubscriptionType.KeepAlive) : base(subscriberId, type)
        {
        }

        public override bool NeedsRenew => true;
    }
}