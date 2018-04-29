using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Finance;
using Prime.Finance.Wallet.Withdrawal.Cancelation;
using Prime.Finance.Wallet.Withdrawal.Confirmation;
using Prime.Finance.Wallet.Withdrawal.History;
using RestEase;

namespace Prime.Finance.Services.Services.BitMex
{
    // https://www.bitmex.com/api/explorer/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class BitMexProvider : IBalanceProvider, IDepositProvider, IWithdrawalProvider, IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> response, [CallerMemberName] string method = "Unknown")
        {
            if (!response.ResponseMessage.IsSuccessStatusCode && response.TryGetContent(out BitMexSchema.ErrorResponse rError))
            {
                throw new ApiResponseException($"{rError.error.name}: {rError.error.message}", this, method);
            }
        }

        public async Task<WalletAddressesResult> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var remoteAssetCode = context.Asset.ToRemoteCode(this);
            var depositAddress = await api.GetUserDepositAddressAsync(remoteAssetCode).ConfigureAwait(false);

            depositAddress = depositAddress.Trim('\"');

            var addresses = new WalletAddressesResult();
            var walletAddress = new WalletAddress(this, context.Asset) { Address = depositAddress };

            addresses.Add(walletAddress);

            return addresses;
        }
        
        public Task<WalletAddressesResult> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();

//            var api = ApiProvider.GetApi(context);
//            var addresses = new WalletAddressesResult();
//
//            foreach (var assetPair in Pairs)
//            {
//                var adjustedCode = AdjustAssetCode(assetPair.Asset1.ShortCode);
//
//                var depositAddress = await api.GetUserDepositAddressAsync(adjustedCode).ConfigureAwait(false);
//
//                depositAddress = depositAddress.Trim('\"');
//
//                // BUG: how to convert XBt from Pairs to BTC?
//                addresses.Add(new WalletAddress(this, Asset.Btc)
//                {
//                    Address = depositAddress
//                });
//            }
//
//            return addresses;
        }
        
        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetUserWalletInfoAsync("XBt").ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var results = new BalanceResults(this);

            var btcAmount = (decimal)ConversionRate * r.amount;

            var c = Asset.Btc;

            results.Add(c, btcAmount, 0);
            return results;
        }
        
        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>();

            if (!String.IsNullOrWhiteSpace(context.AuthenticationToken))
                body.Add("otpToken", context.AuthenticationToken);

            body.Add("currency", context.Amount.Asset.ToRemoteCode(this));
            body.Add("amount", context.Amount.ToDecimalValue() / ConversionRate);
            body.Add("address", context.Address.Address + ":" + context.Address.Tag);

            if (!context.HasCustomFee)
                throw new ContextArgumentException("Custom fee is required for withdrawal", this);

            body.Add("fee", context.CustomFee.Value.ToDecimalValue() / ConversionRate);

            var rRaw = await api.RequestWithdrawalAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }
        
        public async Task<List<WithdrawalHistoryEntry>> GetWithdrawalHistoryAsync(WithdrawalHistoryContext context)
        {
            if (!context.Asset.ToRemoteCode(this).Equals(Asset.Btc.ToRemoteCode(this)))
                throw new ApiBaseException($"{context.Asset.ShortCode} asset is not supported for querying withdrawal history", this);

            var api = ApiProvider.GetApi(context);
            var remoteCode = context.Asset.ToRemoteCode(this);
            var rRaw = await api.GetWalletHistoryAsync(remoteCode).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var history = new List<WithdrawalHistoryEntry>();

            foreach (var rHistory in r.Where(x => x.transactType.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase)))
            {
                history.Add(new WithdrawalHistoryEntry()
                {
                    Price = new Money(rHistory.amount * ConversionRate, context.Asset),
                    Fee = new Money(rHistory.fee * ConversionRate ?? 0.0m, context.Asset),
                    CreatedTimeUtc = rHistory.timestamp,
                    Address = rHistory.address,
                    WithdrawalRemoteId = rHistory.transactID,
                    WithdrawalStatus = ParseWithdrawalStatus(rHistory.transactStatus)
                });
            }

            return history;
        }
        
        private WithdrawalStatus ParseWithdrawalStatus(string statusRaw)
        {
            switch (statusRaw)
            {
                case "Canceled":
                    return WithdrawalStatus.Canceled;
                case "Completed":
                    return WithdrawalStatus.Completed;
                case "Confirmed":
                    return WithdrawalStatus.Confirmed;
                case "Pending":
                    return WithdrawalStatus.Awaiting;
                default:
                    throw new NotImplementedException();
            }
        }
        
        public async Task<WithdrawalCancelationResult> CancelWithdrawalAsync(WithdrawalCancelationContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "token", context.WithdrawalRemoteId }
            };

            var rRaw = await api.CancelWithdrawalAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalCancelationResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }

        public async Task<WithdrawalConfirmationResult> ConfirmWithdrawalAsync(WithdrawalConfirmationContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "token", context.WithdrawalRemoteId }
            };

            var rRaw = await api.ConfirmWithdrawalAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalConfirmationResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var market = context.Pair.ToTicker(this);
            var side = context.IsBuy ? "Buy" : "Sell";

            var rRaw = await api.CreateNewLimitOrderAsync("Limit", 
                side, 
                market, 
                context.Quantity.ToDecimalValue(),
                context.Rate.ToDecimalValue()
                ).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.orderID);
        }

        public async Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            var orders = await GetOrdersAsync(context, false).ConfigureAwait(false);

            return new TradeOrdersResponse(orders);
        }

        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            var orders = await GetOrdersAsync(context, true).ConfigureAwait(false);

            return new OpenOrdersResponse(orders);
        }

        private TradeOrderStatus ParseTradeOrderStatus(BitMexSchema.OrderResponse rOrder)
        {
            var isOpen = rOrder.ordStatus.Equals("new", StringComparison.OrdinalIgnoreCase);
            var isBuy = rOrder.side.Equals("buy", StringComparison.OrdinalIgnoreCase);
            return new TradeOrderStatus(Network, rOrder.orderID, isBuy, isOpen, false)
            {
                AmountInitial = rOrder.orderQty / rOrder.price,
                Rate = rOrder.price,
                Market = rOrder.symbol.ToAssetPair(this, 3)
            };
        }

        private async Task<List<TradeOrderStatus>> GetOrdersAsync(MarketOrdersContext context, bool isOpen)
        {
            var api = ApiProvider.GetApi(context);

            string market = null;

            // Market not required.
            if (context.HasMarket)
                market = context.Market.ToTicker(this);

            var rRaw = await api.GetOrdersAsync(market, isOpen ?  "{\"open\": true}" : null).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.Where(x => !x.ordStatus.Equals("new", StringComparison.OrdinalIgnoreCase)).Select(ParseTradeOrderStatus).ToList();
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetOrdersAsync(filter: $"{{\"orderID\": \"{context.RemoteGroupId}\"}}").ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var rOrders = rRaw.GetContent();

            var rOrder = rOrders.FirstOrDefault(x => x.orderID.Equals(context.RemoteGroupId, StringComparison.Ordinal));
            if(rOrder == null)
                throw new NoTradeOrderException(context, this);

            var isOpen = rOrder.ordStatus.Equals("new", StringComparison.OrdinalIgnoreCase);// rOpenOrders.Exists(x => x.orderID.Equals(context.RemoteGroupId, StringComparison.Ordinal));

            var order = ParseTradeOrderStatus(rOrder);
            order.IsOpen = isOpen;

            return new TradeOrderStatusResponse(order);
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderLimitFeaturesStatic = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderLimitFeaturesStatic;
    }
}