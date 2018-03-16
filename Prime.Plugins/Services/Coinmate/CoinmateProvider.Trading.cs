using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using Prime.Plugins.Services.Yobit;
using RestEase;

namespace Prime.Plugins.Services.Coinmate
{
    public partial class CoinmateProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (!r.TryGetContent(out CoinmateSchema.BaseResponse<T> rError)) 
                return;
            
            if (rError.error)
                throw new ApiResponseException(rError.errorMessage.TrimEnd('.'), this, method);

            if (rError.data == null)
                throw new ApiResponseException("API response error occurred", this, method);
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "currencyPair", context.Pair.ToTicker(this).ToLower() },
                { "amount", context.Quantity.ToDecimalValue()},
                { "price", context.Rate.ToDecimalValue()}
            };

            var rRaw = context.IsBuy
                ? await api.PlaceMarketBuyLimit(body).ConfigureAwait(false)
                : await api.PlaceMarketSellLimit(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.data);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            if(!context.HasMarket)
                throw new MarketNotSpecifiedException(this);
            
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>()
            {
                {"currencyPair", context.Market}
            };

            var rRaw = await api.QueryOrdersAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var order = r.data.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));
            if (order == null)
                throw new NoTradeOrderException(context, this);

            var isOpen = order.status.IndexOf("open", StringComparison.OrdinalIgnoreCase) >= 0;
            var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatus(order.id, isBuy, isOpen, false)
            {
                Rate = order.price,
                AmountInitial = order.originalAmount,
                AmountRemaining = order.remainingAmount,
                Market = context.Market
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            // TODO: AY: Sean implement this method using GetOpenOrders endpoint.
            
            throw new NotImplementedException();
        }

        private async Task<Response<CoinmateSchema.WithdrawalRequestResponse>> SubmitWithdrawalRequestAsync(
            WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"coinName", context.Amount.Asset.ShortCode},
                {"amount", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            if (context.Amount.Asset.Equals(Asset.Btc))
                return await api.SubmitWithdrawRequestBitcoinAsync(body).ConfigureAwait(false);

            if (context.Amount.Asset.Equals(Asset.Ltc))
                return await api.SubmitWithdrawRequestLitecoinAsync(body).ConfigureAwait(false);

            if (context.Amount.Asset.Equals(Asset.Bch))
                return await api.SubmitWithdrawRequestBitcoinCashAsync(body).ConfigureAwait(false);

            throw new ApiBaseException($"Withdrawal of '{context.Amount.Asset}' is not supported by exchange", this);
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var rRaw = await SubmitWithdrawalRequestAsync(context).ConfigureAwait(false);

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.data
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(true, CanGetOrderMarket.ByAdditionalRequest);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
