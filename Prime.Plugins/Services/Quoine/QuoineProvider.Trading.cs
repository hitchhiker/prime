using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Api.Request.Response;
using RestEase;

namespace Prime.Plugins.Services.Quoine
{
    public partial class QuoineProvider : IOrderLimitProvider, IWithdrawalPlacementProvider
    {
        private void CheckResponseErrors<T>(Response<T> rawResponse, [CallerMemberName] string method = "Unknown")
        {
            if (!rawResponse.ResponseMessage.IsSuccessStatusCode)
            {
                if (rawResponse.TryGetContent(out QuoineSchema.ErrorResponse baseResponse))
                {
                    var message = $"Status Code: {baseResponse.StatusCode}. Reason Phrase: {baseResponse.ReasonPhrase}";
                    throw new ApiResponseException(message.TrimEnd('.'), this, method);
                }
                else
                {
                    var reason = rawResponse.ResponseMessage.ReasonPhrase;
                    throw new ApiResponseException($"HTTP error {rawResponse.ResponseMessage.StatusCode} {(String.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}", this, method);
                }
            }
        }

        public Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            throw new NotImplementedException();
        }

        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            throw new NotImplementedException();
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        //public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        //{
        //    var api = ApiProvider.GetApi(context);

        //    var body = new Dictionary<string, object>
        //    {
        //        { "currencyPair", context.Pair.ToTicker(this).ToLower() },
        //        { "amount", context.Quantity.ToDecimalValue()},
        //        { "price", context.Rate.ToDecimalValue()}
        //    };

        //    var rRaw = context.IsBuy
        //        ? await api.PlaceMarketBuyLimit(body).ConfigureAwait(false)
        //        : await api.PlaceMarketSellLimit(body).ConfigureAwait(false);
        //    CheckResponseErrors(rRaw);

        //    var r = rRaw.GetContent();

        //    return new PlacedOrderLimitResponse(r.data);
        //}

        //public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        //{
        //    var api = ApiProvider.GetApi(context);

        //    var body = new Dictionary<string, object>();

        //    var rRaw = await api.QueryOrdersAsync(body).ConfigureAwait(false);
        //    CheckResponseErrors(rRaw);

        //    var r = rRaw.GetContent();

        //    var order = r.data.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));
        //    if (order == null)
        //        throw new NoTradeOrderException(context, this);

        //    var isOpen = order.status.IndexOf("open", StringComparison.OrdinalIgnoreCase) >= 0;
        //    var isBuy = order.type.IndexOf("buy", StringComparison.OrdinalIgnoreCase) >= 0;

        //    return new TradeOrderStatus(order.id, isBuy, isOpen, false)
        //    {
        //        Rate = order.price,
        //        AmountInitial = order.originalAmount,
        //        AmountRemaining = order.remainingAmount,
        //    };
        //}

        //public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => null;

        //private async Task<Response<QuoineSchema.WithdrawalRequestResponse>> SubmitWithdrawalRequestAsync(
        //    WithdrawalPlacementContext context)
        //{
        //    var api = ApiProvider.GetApi(context);

        //    var body = new Dictionary<string, object>
        //    {
        //        {"coinName", context.Amount.Asset.ShortCode},
        //        {"amount", context.Amount.ToDecimalValue()},
        //        {"address", context.Address.Address}
        //    };

        //    if (context.Amount.Asset.Equals(Asset.Btc))
        //        return await api.SubmitWithdrawRequestBitcoinAsync(body).ConfigureAwait(false);

        //    if (context.Amount.Asset.Equals(Asset.Ltc))
        //        return await api.SubmitWithdrawRequestLitecoinAsync(body).ConfigureAwait(false);

        //    if (context.Amount.Asset.Equals(Asset.Bch))
        //        return await api.SubmitWithdrawRequestBitcoinCashAsync(body).ConfigureAwait(false);

        //    throw new ApiBaseException($"Withdrawal of '{context.Amount.Asset}' is not supported by exchange", this);
        //}

        //public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        //{
        //    var rRaw = await SubmitWithdrawalRequestAsync(context);

        //    CheckResponseErrors(rRaw);

        //    var r = rRaw.GetContent();

        //    return new WithdrawalPlacementResult()
        //    {
        //        WithdrawalRemoteId = r.data
        //    };
        //}

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
