using System;
using LiteDB;

namespace Prime.Common.Wallet
{
    /// <summary>
    /// This subscription needs to be renewed at least once per minute.
    /// </summary>
    public class PortfolioSubscribeMessage : RenewableSubscribeMessageBase
    {
        private static readonly TimeSpan ExpirationSpanStatic = TimeSpan.FromMinutes(1);

        public readonly ObjectId UserContextId;

        public PortfolioSubscribeMessage(ObjectId subscriberId, SubscriptionType type = SubscriptionType.KeepAlive) : base(subscriberId, type)
        {
        }

        public PortfolioSubscribeMessage(ObjectId subscriberId, ObjectId userContextId, SubscriptionType type = SubscriptionType.Subscribe) : base(subscriberId, type)
        {
            UserContextId = userContextId;
        }

        public override TimeSpan ExpirationSpan => ExpirationSpanStatic;

        public override bool IsEquivalent(SubscribeMessageBase smb)
        {
            if (!(smb is PortfolioSubscribeMessage o))
                return false;

            return Equals(o.UserContextId, UserContextId);
        }
    }
}