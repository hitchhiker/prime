﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Finance;
using RestEase;

namespace Prime.Finance.Services.Services.BitBay
{
    public partial class BitBayProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.GetContent() is BitBaySchema.ErrorBaseResponse rErrorResponse)
            {
                if (!string.IsNullOrEmpty(rErrorResponse.message))
                    throw new ApiResponseException($"{rErrorResponse.code}: {rErrorResponse.message}", this, method);
            }

            if (!r.ResponseMessage.IsSuccessStatusCode)
                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePostBody("trade");
            body.Add("type", context.IsBuy ? "buy" : "sell");
            body.Add("currency", context.Pair.Asset1.ToRemoteCode(this));
            body.Add("amount", context.Quantity);
            body.Add("payment_currency", context.Pair.Asset2.ToRemoteCode(this));
            body.Add("rate", context.Rate.ToDecimalValue());

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.order_id);
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<BitBaySchema.OrdersResponse> GetOrderResponseByOrderId(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreatePostBody("orders");

            var rRaw = await api.QueryOrdersAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();
            var order = r.FirstOrDefault(x => x.order_id.Equals(context.RemoteGroupId));

            if (order == null)
                throw new NoTradeOrderException(context, this);

            return order;
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var order = await GetOrderResponseByOrderId(context).ConfigureAwait(false);
            var isOpen = order.status.Equals("active", StringComparison.OrdinalIgnoreCase);

            var isBuy = order.type.IndexOf("bid", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.order_id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    Market = new AssetPair(order.order_currency, order.payment_currency, this),
                    Rate = order.current_price,
                    AmountInitial = order.start_price
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var timestamp = (long)DateTime.UtcNow.ToUnixTimeStamp();

            var body = new Dictionary<string, object>
            {
                {"method", "transfer"},
                {"moment", timestamp},
                {"currency", context.Amount.Asset.ShortCode},
                {"quantity", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            var rRaw = await api.SubmitWithdrawRequestAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            // No id is returned from exchange.
            return new WithdrawalPlacementResult();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
