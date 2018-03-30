using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using RestEase;

namespace Prime.Plugins.Services.Binance
{
    // https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md
    public partial class BinanceProvider : IBalanceProvider, IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.TryGetContent(out BinanceSchema.ErrorResponseBase rError))
                if (rError?.success != null && !rError.success.Value)
                    throw new ApiResponseException($"{rError.msg.Trim('.')}", this, method);
            
            if (r.ResponseMessage.IsSuccessStatusCode) return;

            if (rError != null && !string.IsNullOrWhiteSpace(rError.msg))
                throw new ApiResponseException($"{rError.msg} ({rError.code})", this, method);

            throw new ApiResponseException(r.ResponseMessage.ReasonPhrase, this, method);
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAccountInformationAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var balances = new BalanceResults(this);

            foreach (var b in r.balances)
            {
                var asset = b.asset.ToAsset(this);
                balances.Add(asset, b.free, b.locked);
            }

            return balances;
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.NewOrderAsync(context.Pair.ToTicker(this), context.IsBuy ? "BUY" : "SELL", "LIMIT", "GTC",
                context.Quantity, context.Rate).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.orderId.ToString());
        }

        public async Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            if (!context.HasMarket)
                throw new MarketNotSpecifiedException(this);

            var ticker = context.Market.ToTicker(this);
            var api = ApiProvider.GetApi(context);
            
            var rRaw = await api.GetAllOrdersAsync(ticker).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var rOrders = rRaw.GetContent();
            
            var orders = new List<TradeOrderStatus>();

            foreach (var rOrder in rOrders)
            {
                var isBuy = rOrder.side.Equals("BUY", StringComparison.OrdinalIgnoreCase);
                var isOpen = rOrder.status.Equals("NEW", StringComparison.OrdinalIgnoreCase) || rOrder.status.Equals("PARTIALLY_FILLED", StringComparison.OrdinalIgnoreCase);
                var isCancelRequested = rOrder.status.Equals("PENDING_CANCEL", StringComparison.OrdinalIgnoreCase);
                orders.Add(new TradeOrderStatus(Network, rOrder.orderId.ToString(), isBuy, isOpen, isCancelRequested)
                {
                    AmountInitial = rOrder.origQty,
                    Rate = rOrder.price,
                    AmountRemaining = rOrder.origQty - rOrder.executedQty,
                    Market = context.Market
                });
            }
            
            return new TradeOrdersResponse(orders)
            {
                ApiHitsCount = 5 // From API docs: Weight = 5.
            };
        }

        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);
            
            var rRaw = context.HasMarket 
                ? await api.GetOpenOrdersAsync(context.Market.ToTicker(this)).ConfigureAwait(false)
                : await api.GetOpenOrdersAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var rOrders = rRaw.GetContent();

            var orders = new List<TradeOrderStatus>();

            foreach (var rOrder in rOrders)
            {
                var isBuy = rOrder.side.Equals("BUY", StringComparison.OrdinalIgnoreCase);
                var isOpen = rOrder.status.Equals("NEW", StringComparison.OrdinalIgnoreCase) || rOrder.status.Equals("PARTIALLY_FILLED", StringComparison.OrdinalIgnoreCase);
                var isCancelRequested = rOrder.status.Equals("PENDING_CANCEL", StringComparison.OrdinalIgnoreCase);
                orders.Add(new TradeOrderStatus(Network, rOrder.orderId.ToString(), isBuy, isOpen, isCancelRequested)
                {
                    AmountInitial = rOrder.origQty,
                    Rate = rOrder.price,
                    AmountRemaining = rOrder.origQty - rOrder.executedQty,
                    Market = new AssetPair(rOrder.symbol)
                });
            }
            
            return new OpenOrdersResponse(orders)
            {
                ApiHitsCount = 1
            };
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            if(!context.HasMarket)
                throw new MarketNotSpecifiedException(this);

            var api = ApiProvider.GetApi(context);

            if(!long.TryParse(context.RemoteGroupId, out var orderId))
                throw new ApiResponseException("Incorrect order ID specified - must be a number", this);

            var rRaw = await api.QueryOrderAsync(context.Market.ToTicker(this), orderId).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var isCancelRequested = r.status.Equals("pending_cancel", StringComparison.OrdinalIgnoreCase);
            var isOpen = r.status.Equals("new", StringComparison.OrdinalIgnoreCase);

            var isBuy = r.side.Equals("buy", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatusResponse(Network, r.orderId.ToString(), isBuy, isOpen, isCancelRequested)
            {
                TradeOrderStatus =
                {
                    Rate = r.price,
                    AmountInitial = r.origQty,
                    AmountRemaining = r.origQty - r.executedQty
                }
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        [Obsolete("To be implemented soon.")]
        public async Task GetDepositHistoryAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetDepositHistoryAsync().ConfigureAwait(false);

            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.FromNowhere)
        {
            MarketByOrderRequestAffectsRateLimiter = true
        };
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        // When 22 XRP is submitted - 21.75 will be sent.
        public bool IsWithdrawalFeeIncluded => true;

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.SubmitWithdrawRequestAsync(context.Amount.Asset.ShortCode, context.Address.Address,
                context.Amount.ToDecimalValue(), context.Description, "Primary test").ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.id
            };
        }
    }
}
