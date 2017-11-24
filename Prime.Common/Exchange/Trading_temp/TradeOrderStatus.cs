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

        public bool IsClosed => IsFound && !IsOpen;
        public bool IsCanceled => IsCancelRequested && IsClosed;
    }
}