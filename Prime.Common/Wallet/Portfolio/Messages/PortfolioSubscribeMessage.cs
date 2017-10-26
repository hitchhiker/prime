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

        public PortfolioSubscribeMessage(ObjectId subscriberId, SubscriptionType type = SubscriptionType.KeepAlive) : base(subscriberId, type)
        {
        }

        public override TimeSpan ExpirationSpan => ExpirationSpanStatic;

        public override bool IsEquivalent(SubscribeMessageBase smb)
        {
            return true;
        }
    }
}