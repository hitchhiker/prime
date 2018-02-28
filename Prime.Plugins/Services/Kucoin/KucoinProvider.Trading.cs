using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Kucoin
{
    public partial class KucoinProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.GetContent() is KucoinSchema.ErrorBaseResponse rError)
            {
                if (!string.IsNullOrEmpty(rError.code) && !rError.success)
                    throw new ApiResponseException($"{rError.code}: {rError.msg}", this, method);

                if (!string.IsNullOrEmpty(rError.error))
                    throw new ApiResponseException($"{rError.error}: {rError.message}", this, method);
            }

            if (!r.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.NewOrderAsync(context.Pair.ToTicker(this), context.IsBuy ? "BUY" : "SELL", context.Rate, context.Quantity).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.data.orderOid);
        }

        [Obsolete("Sean please review this method and consider using 'OrderDetails' endpoint", false)]
        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            if(context.Market == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var ticker = context.Market.ToTicker(this);

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(ticker).ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var rDealtOrdersRaw = await api.QueryDealtOrdersAsync(ticker).ConfigureAwait(false);
            CheckResponseErrors(rDealtOrdersRaw);

            var rDealtOrders = rDealtOrdersRaw.GetContent();

            var activeOrderBuy = rActiveOrders.data.BUY.FirstOrDefault(x => x.oid.Equals(context.RemoteGroupId));
            var activeOrderSell = rActiveOrders.data.SELL.FirstOrDefault(x => x.oid.Equals(context.RemoteGroupId));
            var dealtOrder = rDealtOrders.data.datas.FirstOrDefault(x => x.orderOid.Equals(context.RemoteGroupId));

            var price = 0m;
            var amountInitial = 0m;
            var isOpen = true;

            if (activeOrderBuy != null)
            {
                price = activeOrderBuy.price;
            }
            else if (activeOrderSell != null)
            {
                price = activeOrderSell.price;
            }
            else if (dealtOrder != null)
            {
                price = dealtOrder.dealPrice;
                isOpen = false;
            }
            else
            {
                throw new NoTradeOrderException(context, this);
            }

            return new TradeOrderStatus(context.RemoteGroupId, isOpen, false)
            {
                Rate = price,
                AmountInitial = amountInitial
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.SubmitWithdrawRequestAsync(context.Amount.Asset.ShortCode, context.Amount.ToDecimalValue(), context.Address.Address).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned from exchange.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        // Note: supports only dealt orders getting without market being specified.
        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, false); 
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
