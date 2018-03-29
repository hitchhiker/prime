using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using Prime.Plugins.Services.Yobit;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitKonan
{
    public partial class BitKonanProvider : IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out BitKonanSchema.BaseResponse rError))
            {
                if (rError.errors?.Length > 0)
                    throw new ApiResponseException(rError.errors[0].TrimEnd('.'), this, method);

                if (rError.status.Equals("failed",StringComparison.OrdinalIgnoreCase))
                    throw new ApiResponseException("API response error occurred", this, method);
            }
        }

        //TODO: SC: This method still needs to be tested with a balance greater than 0 in the account, to see what it returns (documentation does not explain). It should return the order ID.
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "pair", context.Pair.ToTicker(this, '/').ToUpper() },
                { "side", context.IsBuy ? "BUY" : "SELL"},
                { "type", "LIMIT"},
                { "amount", context.Quantity.ToDecimalValue()},
                { "limit", context.Rate.ToDecimalValue()}
            };

            var rRaw = await api.NewOrderAsync(body).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.data?.order_id);
        }

        public Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rActiveOrdersRaw = await api.QueryActiveOrdersAsync().ConfigureAwait(false);
            CheckResponseErrors(rActiveOrdersRaw);

            var rActiveOrders = rActiveOrdersRaw.GetContent();

            var activeOrder = rActiveOrders.data.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));

            if (activeOrder == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = activeOrder.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatusResponse(Network, activeOrder.id, isBuy, true, false)
            {
                TradeOrderStatus =
                {
                    Rate = activeOrder.price,
                    AmountInitial = activeOrder.amount,
                    Market = activeOrder.trade_pair.ToAssetPair(this, '/')
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;
    }
}
