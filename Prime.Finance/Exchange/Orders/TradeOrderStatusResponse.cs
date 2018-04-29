using Prime.Core;

namespace Prime.Finance
{
    public class TradeOrderStatusResponse : ResponseModelBase
    {
        public TradeOrderStatus TradeOrderStatus { get; }

        public TradeOrderStatusResponse(TradeOrderStatus tradeOrderStatus)
        {
            TradeOrderStatus = tradeOrderStatus;
        }

        public TradeOrderStatusResponse(Network network, string remoteOrderId, bool isBuy, bool isOpen,
            bool isCancelRequested)
        {
            TradeOrderStatus = new TradeOrderStatus(network, remoteOrderId, isBuy, isOpen, isCancelRequested);
        }
    }
}