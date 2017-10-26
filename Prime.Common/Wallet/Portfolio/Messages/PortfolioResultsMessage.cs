using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common.Wallet
{
    public class PortfolioResultsMessage
    {
        public DateTime UtcLastUpdated { get; internal set; }

        public bool IsCollectionComplete { get; internal set; }
        public bool IsConversionComplete { get; internal set; }

        public IReadOnlyList<PortfolioLineItem> Items { get; internal set; }
        public IReadOnlyList<PortfolioNetworkInfoItem> NetworkItems { get; internal set; }
        public IReadOnlyList<PortfolioGroupedItem> GroupedAsset { get; internal set; }

        public IReadOnlyList<IWalletService> WorkingProviders { get; internal set; }
        public IReadOnlyList<IWalletService> QueryingProviders { get; internal set; }
        public IReadOnlyList<IWalletService> FailingProviders { get; internal set; }

        public Money TotalConverted { get; internal set; }
    }
}