using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Xbtce
{
    public partial class XbtceProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (!r.ResponseMessage.IsSuccessStatusCode)
            {
                if (r.TryGetContent(out XbtceSchema.ErrorResponse rError))
                    throw new ApiResponseException($"{rError.Message})", this, method);

                throw new ApiResponseException($"{r.ResponseMessage.ReasonPhrase} ({r.ResponseMessage.StatusCode})",
                    this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "Symbol", context.Pair.ToTicker(this) },
                { "Side", context.IsBuy ? "Buy" : "Sell"},
                {"Type", "Limit" },
                { "Amount", context.Quantity.ToDecimalValue()},
                { "Price", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.Id);
        }

        public Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.QueryOrderAsync(context.RemoteGroupId).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (r == null)
                throw new NoTradeOrderException(context, this);

            var isOpen = r.Status.IndexOf("New", StringComparison.OrdinalIgnoreCase) >= 0;
            var isBuy = r.Side.IndexOf("Buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, r.Id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    Rate = r.Price,
                    AmountInitial = r.InitialAmount,
                    AmountRemaining = r.RemainingAmount,
                    Market = r.Symbol
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
