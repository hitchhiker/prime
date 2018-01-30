using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using RestEase;

namespace Prime.Plugins.Services.Kucoin
{
    public partial class KucoinProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            var rError = r.GetContent() as KucoinSchema.BaseResponse<object>;
            if (rError?.success == false)
                throw new ApiResponseException($"{rError.msg}", this, method);

            if (r.ResponseMessage.IsSuccessStatusCode) return;

            if (rError != null && !string.IsNullOrWhiteSpace(rError.msg))
                throw new ApiResponseException($"{rError.msg} ({rError.code})", this, method);

            throw new ApiResponseException(r.ResponseMessage.ReasonPhrase, this, method);
        }

        //TODO: Not tested with real money
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.NewOrderAsync(context.Pair.ToTicker(this), context.IsBuy ? "BUY" : "SELL", context.Rate, context.Quantity).ConfigureAwait(false);

            if (r.success == false)
            {
                throw new ApiResponseException(r.msg, this);
            }

            return new PlacedOrderLimitResponse(r.data.orderOid);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (!context.HasMarket)
                throw new ApiResponseException("Market should be specified when querying order status", this);

            if (string.IsNullOrWhiteSpace(context.RemoteGroupId))
                throw new ApiResponseException("Order ID not specified", this);

            var rActiveOrders = await api.QueryActiveOrdersAsync(context.Market.ToTicker(this)).ConfigureAwait(false);

            if (rActiveOrders.success == false)
            {
                throw new ApiResponseException(rActiveOrders.msg, this);
            }

            var rDealtOrders = await api.QueryDealtOrdersAsync(context.Market.ToTicker(this)).ConfigureAwait(false);

            if (rDealtOrders.success == false)
            {
                throw new ApiResponseException(rDealtOrders.msg, this);
            }

            var activeOrderBuy = rActiveOrders.data.BUY.FirstOrDefault(x => x.oid.Equals(context.RemoteGroupId));
            var activeOrderSell = rActiveOrders.data.SELL.FirstOrDefault(x => x.oid.Equals(context.RemoteGroupId));
            var dealtOrder = rDealtOrders.data.datas.FirstOrDefault(x => x.orderOid.Equals(context.RemoteGroupId));

            decimal price;
            decimal amountInitial = 0;
            decimal amountRemaining = 0;
            bool isOpen = false;

            if (activeOrderBuy != null)
            {
                isOpen = true;
                price = activeOrderBuy.price;
            }
            else if (activeOrderSell != null)
            {
                isOpen = true;
                price = activeOrderSell.price;
            }
            else if (dealtOrder != null)
            {
                price = dealtOrder.dealPrice;
            }
            else
            {
                price = 0;
            }

            return new TradeOrderStatus(context.RemoteGroupId, isOpen, false)
            {
                Rate = price,
                AmountInitial = new Money(amountInitial, context.Market.Asset1),
                AmountRemaining = new Money(amountRemaining, context.Market.Asset1),
            };
        }

        //TODO: Not tested with real money
        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.SubmitWithdrawRequestAsync(context.Amount.Asset.ShortCode, context.Amount.ToDecimalValue(), context.Address.Address).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
