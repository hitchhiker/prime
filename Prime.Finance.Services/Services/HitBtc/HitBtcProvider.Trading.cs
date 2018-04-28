using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Common;
using RestEase;

namespace Prime.Finance.Services.Services.HitBtc
{
    // https://api.hitbtc.com/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class HitBtcProvider : IBalanceProvider, IDepositProvider, IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if(r.ResponseMessage.IsSuccessStatusCode) return;

            if (r.TryGetContent(out HitBtcSchema.BaseResponse rError) && rError.error != null)
                throw new ApiResponseException($"{rError.error.message.Trim(".")} ({rError.error.code}){ (string.IsNullOrWhiteSpace(rError.error.description) ? "" : $": { rError.error.description.Trim(".") }") }", this, method);
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetTradingBalanceAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var balances = new BalanceResults(this);

            foreach (var rBalance in r)
                balances.Add(rBalance.currency.ToAsset(this), rBalance.available, rBalance.reserved);

            return balances;
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pair = context.Pair.ToTicker(this);

            var body = CreateHitBtcRequestBody();
            body.Add("symbol", pair);
            body.Add("side", context.IsBuy ? "buy" : "sell");
            body.Add("quantity", context.Quantity.ToDecimalValue());
            body.Add("price", context.Rate.ToDecimalValue());
            body.Add("type", "limit");
            body.Add("timeInForce", "GTC");
            body.Add("strictValidate", "false");

            var rRaw = await api.CreateNewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.clientOrderId);
        }

        public async Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = context.HasMarket
                ? await api.GetOrdersHistoryAsync(context.Market.ToTicker(this).ToUpper()).ConfigureAwait(false)
                : await api.GetOrdersHistoryAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var orders = new List<TradeOrderStatus>();

            foreach (var rOrder in r)
            {
                var order = ParseOrderResponse(rOrder);

                if (context.HasMarket)
                    order.Market = context.Market;

                orders.Add(order);
            }

            return new TradeOrdersResponse(orders);
        }

        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = context.HasMarket
                ? await api.GetOpenOrdersAsync(context.Market.ToTicker(this).ToUpper()).ConfigureAwait(false)
                : await api.GetOpenOrdersAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var orders = new List<TradeOrderStatus>();

            foreach (var rOrder in r)
            {
                var order = ParseOrderResponse(rOrder);

                if (context.HasMarket)
                    order.Market = context.Market;

                orders.Add(order);
            }

            return new OpenOrdersResponse(orders);
        }

        private TradeOrderStatus ParseOrderResponse(HitBtcSchema.OrderInfoResponse rOrder)
        {
            var isOpen = rOrder.status.Equals("new", StringComparison.OrdinalIgnoreCase);
            var isCancelRequested = rOrder.status.Equals("canceled", StringComparison.OrdinalIgnoreCase);

            var isBuy = rOrder.side.Equals("buy", StringComparison.OrdinalIgnoreCase);
            return new TradeOrderStatus(Network, rOrder.clientOrderId, isBuy, isOpen, isCancelRequested)
            {
                Rate = rOrder.price,
                AmountInitial = rOrder.quantity
            };
        }

        private Dictionary<string, object> CreateHitBtcRequestBody() => new Dictionary<string, object>();

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetActiveOrderInfoAsync(context.RemoteGroupId).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var isOpen = r.status.Equals("new", StringComparison.OrdinalIgnoreCase);
            var isCancelRequested = r.status.Equals("canceled", StringComparison.OrdinalIgnoreCase);

            var isBuy = r.side.Equals("buy", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatusResponse(Network, r.clientOrderId, isBuy, isOpen, isCancelRequested)
            {
                TradeOrderStatus =
                {
                    Rate = r.price,
                    AmountInitial = r.quantity
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        /// <summary>
        /// CanGetOrderMarket.FromNowhere because order's market is non-parseable.
        /// </summary>
        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        // When 50 XRP are submitted, 49.491000 XRP will be received.
        public bool IsWithdrawalFeeIncluded => true;
        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreateHitBtcRequestBody();
            body.Add("currency", context.Amount.Asset.ShortCode);
            body.Add("amount", context.Amount.ToDecimalValue());
            body.Add("address", context.Address.Address);

            if(context.HasDescription)
                body.Add("paymentId", context.Description);
            if(context.HasCustomFee)
                body.Add("networkFee", context.CustomFee.Value.ToDecimalValue());

            // body.Add("includeFee", ""); // Not analyzed and checked.
            body.Add("autoCommit", true);

            var rRaw = await api.WithdrawCryptoAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.id
            };
        }
    }
}
