using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;

namespace Prime.Plugins.Services.BitMex
{
    public partial class BitMexProvider : IBalanceProvider, IDepositProvider, IWithdrawalProvider, IOrderLimitProvider
    {
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

            var r = await api.GetUserWalletInfoAsync("XBt").ConfigureAwait(false);

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

            var r = await api.RequestWithdrawalAsync(body).ConfigureAwait(false);

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
            var r = await api.GetWalletHistoryAsync(remoteCode).ConfigureAwait(false);

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

            var r = await api.CancelWithdrawalAsync(body).ConfigureAwait(false);

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

            var r = await api.ConfirmWithdrawalAsync(body).ConfigureAwait(false);

            return new WithdrawalConfirmationResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }

        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrdersResponse> GetTradeOrdersAsync(TradeOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public MinimumTradeVolume[] MinimumTradeVolume { get; }
        public OrderLimitFeatures OrderLimitFeatures { get; }
    }
}