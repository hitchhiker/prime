using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Prime.Common.Exchange.Trading_temp
{
    public class TradeTargetPrice : TradeStrategyBase
    {
        private readonly TradeTargetPriceContext _ctx;

        public TradeTargetPrice(TradeTargetPriceContext context) : base(context)
        {
            _ctx = context;
        }

        public override void Start()
        {
            base.Start();

            var price = _ctx.Price;

            if (_ctx.IsTesting)
                price = _ctx.IsBuy ? new Money(price * .9m, _ctx.Price.Asset) : new Money(price * 1.1m, _ctx.Price.Asset);

            var r = AsyncContext.Run(()=> Provider.PlaceOrderLimitAsync(new PlaceOrderLimitContext(_ctx.UserContext, _ctx.Pair, _ctx.IsBuy, _ctx.Quantity, price)));
            RemoteTradeId = r.RemoteOrderGroupId;

            Task.Run(()=> WaitForEnd(r.RemoteOrderGroupId));
        }

        private async void WaitForEnd(string tradeId)
        {
            TradeOrderStatus lastStatus = null;
            var finished = false;
            do
            {
                lastStatus = await Provider.GetOrderStatusAsync(new RemoteMarketIdContext(Context.UserContext, tradeId)).ConfigureAwait(false);
                finished = finished || lastStatus.IsClosed;
                if (!finished)
                    Thread.Sleep(1000);
            } while (!finished);

            SetComplete();
        }
    }
}
