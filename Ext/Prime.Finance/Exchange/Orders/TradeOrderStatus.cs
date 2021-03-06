﻿using Prime.Core;

namespace Prime.Finance
{
    /// <summary>
    /// Contains information for specific order.
    /// </summary>
    public class TradeOrderStatus
    {
        public TradeOrderStatus() {}

        public TradeOrderStatus(Network network, string remoteOrderId, bool isBuy, bool isOpen, bool isCancelRequested)
        {
            Network = network;
            IsFound = true;
            RemoteOrderId = remoteOrderId;
            IsBuy = isBuy;
            IsOpen = isOpen;
            IsCancelRequested = isCancelRequested;

            Market = AssetPair.Empty;
        }
        
        public Network Network { get; }

        public bool IsBuy { get; }
        public bool IsSell => !IsBuy;

        public AssetPair Market { get; set; }
        public bool HasMarket => !Equals(Market, AssetPair.Empty);

        public bool IsFound { get; }
        public string RemoteOrderId { get; }
        public bool IsOpen { get; set; }
        public bool IsCancelRequested { get; }

        public decimal? Rate { get; set; }
        public decimal? AmountInitial { get; set; }
        public decimal? AmountRemaining { get; set; }

        public decimal? AmountFilled => AmountInitial.HasValue && AmountRemaining.HasValue ? AmountInitial.Value - AmountRemaining.Value : (decimal?) null;

        public bool IsClosed => IsFound && !IsOpen;
        public bool IsCanceled => IsCancelRequested && IsClosed;
    }
}