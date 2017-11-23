namespace Prime.Common
{
    public class TradeOrderStatus
    {
        public TradeOrderStatus() {}

        public TradeOrderStatus(string remoteOrderId, bool isOpen)
        {
            IsFound = true;
            RemoteOrderId = remoteOrderId;
            IsOpen = isOpen;
        }

        public bool IsFound { get; }
        public string RemoteOrderId { get; }
        public bool IsOpen { get; }

        public bool IsClosed => IsFound && !IsOpen;
    }
}