using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Finance.Exchange.Trading_temp.Messages;

namespace Prime.Finance
{
    public class TradingMessenger : IStartupMessenger
    {
        public IMessenger M { get; private set; }

        public void Start(ServerContext context)
        {
            M = context.M;
            M.RegisterAsync<RequestTradeMessage>(this, RequestTradeMessage);
            M.RegisterAsync<TradeStatusChangedMessage>(this, TradeStatusChangedMessage);
        }

        public void Stop()
        {
            M.UnregisterAsync(this);
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