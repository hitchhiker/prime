using System;
using LiteDB;

namespace Prime.Core
{
    public abstract class RenewableSubscribeMessageBase : SubscribeMessageBase
    {
        internal bool Unsubscribe;

        protected RenewableSubscribeMessageBase(ObjectId subscriberId, SubscriptionType type = SubscriptionType.KeepAlive) : base(subscriberId, type)
        {
        }

        public override bool NeedsRenew => true;
    }
}