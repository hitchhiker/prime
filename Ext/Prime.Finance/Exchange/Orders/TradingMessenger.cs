using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Finance.Exchange.Trading_temp.Messages;

namespace Prime.Finance
{
    public class TradingMessenger : IStartupMessenger
    {
        private readonly IMessenger _m = DefaultMessenger.I.Default;

        public TradingMessenger()
        {
            _m.RegisterAsync<RequestTradeMessage>(this, RequestTradeMessage);
            _m.RegisterAsync<TradeStatusChangedMessage>(this, TradeStatusChangedMessage);
        }

        private void RequestTradeMessage(RequestTradeMessage m)
        {
            TradingCoordinator.I.Process(m.Strategy);
        }

        private void TradeStatusChangedMessage(TradeStatusChangedMessage m)
        {
            TradingCoordinator.I.Changed(m.TradeId);
        }
    }
}