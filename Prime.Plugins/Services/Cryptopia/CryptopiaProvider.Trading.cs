using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;

namespace Prime.Plugins.Services.Cryptopia
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://www.cryptopia.co.nz/Forum/Thread/255
    public partial class CryptopiaProvider : IOrderLimitProvider, IBalanceProvider, IWithdrawalPlacementProvider
    {
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.SubmitTradeRequest
            {
                Market = context.Pair.ToTicker(this, "/"),
                Type = context.IsBuy ? "Buy" : "Sell",
                Rate = context.Rate,
                Amount = context.Quantity
            };

            var rRaw = await api.SubmitTradeAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();
            return new PlacedOrderLimitResponse(r.Data.OrderId.ToString());
        }

        private async Task<IEnumerable<TradeOrderStatus>> GetOpenOrdersAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.GetOpenOrdersRequest
            {
                Count = 1000,
                Market = context.Market.ToTicker(this, "/")
            };

            var rRaw = await api.GetOpenOrdersAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.Data.Select(x => new TradeOrderStatus(x.OrderId.ToString(), true, false));
        }

        private async Task<IEnumerable<TradeOrderStatus>> GetTradeHistoryAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.GetTradeHistoryRequest
            {
                Count = 1000,
                Market = context.Market.ToTicker(this, "/")
            };

            var rRaw = await api.GetTradeHistoryAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return r.Data.Select(x => new TradeOrderStatus(x.TradeId.ToString(), false, false));
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var openOrders = await GetOpenOrdersAsync(context).ConfigureAwait(false);

            var order = openOrders.FirstOrDefault(x => x.RemoteOrderId.Equals(context.RemoteGroupId));

            var isOpen = true;

            if (order == null)
            {
                var tradeHistory = await GetTradeHistoryAsync(context).ConfigureAwait(false);

                var trade = tradeHistory.FirstOrDefault(x => x.RemoteOrderId.Equals(context.RemoteGroupId));

                if(trade == null)
                    throw new NoTradeOrderException(context, this);

                isOpen = false;
            }

            return new TradeOrderStatus(context.RemoteGroupId, isOpen, false);
        }

        // TODO: AY: find out MinimumTradeVolume in Cryptopia.
        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetBalanceAsync(new CryptopiaSchema.BalanceRequest()).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();
            
            var balances = new BalanceResults(this);

            foreach (var rBalance in r.Data)
            {
                var asset = rBalance.Symbol.ToAsset(this);
                balances.Add(asset, rBalance.Available, rBalance.HeldForTrades + rBalance.PendingWithdraw);
            }

            return balances;
        }

        // TODO: AY: find out IsFeeIncluded in Cryptopia.
        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new CryptopiaSchema.SubmitWithdrawRequest
            {
                Address = context.Address.Address,
                Amount = context.Amount,
                Currency = context.Amount.Asset.ToRemoteCode(this),
                PaymentId = context.Description
            };

            var rRaw = await api.SubmitWithdrawAsync(body).ConfigureAwait(false);

            CheckCryptopiaResponseErrors(rRaw);

            var r = rRaw.GetContent();

            if (!r.Data.HasValue)
                throw new ApiResponseException("Remote withdrawal ID is not returned", this);

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.Data.ToString()
            };
        }
    }
}
