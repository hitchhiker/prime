using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Prime.Utility;

namespace Prime.Common.Wallet.Portfolio.Messages
{
    public class ExchangePortfolioChangedMessage
    {
        public readonly decimal Amount;
        public readonly decimal Percentage;
        public readonly ObjectId UserId;
        public readonly DateTime UtcLastUpdated;
        public readonly PortfolioInfoItem PortfolioInfoItem;

        public ExchangePortfolioChangedMessage(ObjectId userId, decimal amount, decimal percentage, DateTime utcLastUpdated, PortfolioInfoItem portfolioInfoItem)
        {
            Amount = amount;
            Percentage = percentage;
            UserId = userId;
            UtcLastUpdated = utcLastUpdated;
            PortfolioInfoItem = portfolioInfoItem;
        }
    }
}
