using System;
using LiteDB;

namespace Prime.Common
{
    public abstract class SubscribeMessageBase
    {
        public readonly ObjectId SubscriberId;
        public readonly SubscriptionType SubscriptionType;

        protected SubscribeMessageBase(ObjectId subscriberId, SubscriptionType subscriptionType)
        {
            SubscriberId = subscriberId;
            SubscriptionType = subscriptionType;
        }

        public abstract bool IsEquivalent(SubscribeMessageBase smb);
        public abstract TimeSpan ExpirationSpan { get; }
        public virtual bool NeedsRenew => false;
    }
}