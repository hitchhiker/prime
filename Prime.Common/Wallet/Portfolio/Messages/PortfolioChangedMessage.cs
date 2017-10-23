using System;
using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Common.Wallet
{
    public class PortfolioChangedMessage
    {
        public readonly ObjectId UserId;
        public readonly DateTime UtcLastUpdated;
        public readonly UniqueList<PortfolioLineItem> Items;
        public readonly UniqueList<PortfolioInfoItem> InfoItems;
        public readonly List<IWalletService> Working;
        public readonly List<IWalletService> Querying;
        public readonly List<IWalletService> Failing;

        public PortfolioChangedMessage(ObjectId userId, DateTime utcLastUpdated, UniqueList<PortfolioLineItem> items, UniqueList<PortfolioInfoItem> infoItems, List<IWalletService> working, List<IWalletService> querying, List<IWalletService> failing)
        {
            UserId = userId;
            UtcLastUpdated = utcLastUpdated;
            Items = items;
            InfoItems = infoItems;
            Working = working;
            Querying = querying;
            Failing = failing;
        }
    }
}