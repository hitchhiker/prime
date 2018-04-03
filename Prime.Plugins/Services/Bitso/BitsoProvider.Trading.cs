using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Bitso
{
    public partial class BitsoProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                var reason = rawResponse.ResponseMessage.ReasonPhrase;

                if (rawResponse.TryGetContent(out BitsoSchema.ErrorResponse baseResponse))
                {
                    throw new ApiResponseException($"Error Code: {baseResponse.error.code} - {baseResponse.error.message.TrimEnd('.')}", this, method);

                }

                throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            if (context.Pair == null)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            var side = context.IsBuy ? "buy" : "sell";

            var body = new Dictionary<string, object>
            {
                { "book", context.Pair.ToTicker(this).ToLower()},
                { "side", side},
                { "price", context.Rate.ToDecimalValue().ToString()},
                { "major", context.Quantity.ToDecimalValue()}, 
                { "type",  "limit"  }
            };

            var rRaw = await api.PlaceNewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.payload.oid);
        }

        public Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var ordersRaw = await api.QueryOrderAsync(context.RemoteGroupId).ConfigureAwait(false);

            CheckResponseErrors(ordersRaw);

            var rOrders = ordersRaw.GetContent();

            if (rOrders.payload.Length == 0)
            {
                throw new NoTradeOrderException(context, this);
            }

            var order = rOrders.payload[0];
        
            var isBuy = order.side.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;
            var isOpen = order.status.IndexOf("open", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, order.oid, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    AmountInitial = order.original_amount,
                    AmountRemaining = order.unfilled_amount,
                    Rate = order.price,
                    Market = order.book.ToAssetPair(this)
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<Response<BitsoSchema.WithdrawalRequestResponse>> SubmitWithdrawalRequestAsync(
            WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"amount", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            if (context.Amount.Asset.Equals(Asset.Btc))
                return await api.SubmitWithdrawRequestBtcAsync(body).ConfigureAwait(false);

            if (context.Amount.Asset.Equals(Asset.Ltc))
                return await api.SubmitWithdrawRequestLtcAsync(body).ConfigureAwait(false);

            if (context.Amount.Asset.Equals(Asset.Bch))
                return await api.SubmitWithdrawRequestBchAsync(body).ConfigureAwait(false);

            if (context.Amount.Asset.Equals(Asset.Eth))
                return await api.SubmitWithdrawRequestEthAsync(body).ConfigureAwait(false);

            if (context.Amount.Asset.Equals(Asset.Xrp))
                return await api.SubmitWithdrawRequestXrpAsync(body).ConfigureAwait(false);

            throw new ApiBaseException($"Withdrawal of '{context.Amount.Asset}' is not supported by exchange", this);
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var rRaw = await SubmitWithdrawalRequestAsync(context).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.payload.wid
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
