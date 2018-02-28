namespace Prime.Common
{
    public class TradeOrderStatus
    {
        public TradeOrderStatus() {}

        public TradeOrderStatus(string remoteOrderId, bool isOpen, bool isCancelRequested)
        {
            IsFound = true;
            RemoteOrderId = remoteOrderId;
            IsOpen = isOpen;
            IsCancelRequested = isCancelRequested;
        }

        public bool IsFound { get; }
        public string RemoteOrderId { get; }
        public bool IsOpen { get; }
        public bool IsCancelRequested { get; }

        public decimal? Rate { get; set; }
        public decimal? AmountInitial { get; set; }
        public decimal? AmountRemaining { get; set; }

        public decimal? AmountFilled => AmountInitial.HasValue && AmountRemaining.HasValue ? AmountInitial.Value - AmountRemaining.Value : (Money?) null;

        public bool IsClosed => IsFound && !IsOpen;
        public bool IsCanceled => IsCancelRequested && IsClosed;
    }
}