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

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            if (!context.HasMarket)
                throw new ApiResponseException("Market should be specified when querying order status", this);

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync(context.Market.ToTicker(this)).ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var rDealtOrdersRaw = await api.QueryDealtOrdersAsync(context.Market.ToTicker(this)).ConfigureAwait(false);
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
                AmountInitialNumeric = new Money(amountInitial, context.Market.Asset2)
            };
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.SubmitWithdrawRequestAsync(context.Amount.Asset.ShortCode, context.Amount.ToDecimalValue(), context.Address.Address).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
