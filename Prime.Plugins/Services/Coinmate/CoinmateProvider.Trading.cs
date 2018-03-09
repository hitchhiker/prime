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
            if (r.TryGetContent(out CoinmateSchema.BaseResponse<T> rError))
            {
                if (rError.error)
                {
                    throw new ApiResponseException(rError.errorMessage, this, method);
                }

                if (rError.data == null)
                {
                    throw new ApiResponseException("API response error occurred", this, method);
                }
            }
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

            Response<CoinmateSchema.NewOrderResponse> rRaw;

            if (context.IsBuy)
            {
                rRaw = await api.PlaceMarketBuyLimit(body).ConfigureAwait(false);
            }
            else
            {
                rRaw = await api.PlaceMarketSellLimit(body).ConfigureAwait(false);
            }

            CheckResponseErrors(rRaw);
            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.data);
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            string market = context.Market.ToTicker(this).ToLower();

            var order = await GetOrderReponseByIdAsync(context, market).ConfigureAwait(false);

            var isOpen = order.status.IndexOf("OPEN", StringComparison.OrdinalIgnoreCase) >= 0;

            var isBuy = order.type.IndexOf("BUY", StringComparison.OrdinalIgnoreCase) >= 0;

            return new TradeOrderStatus(order.id, isBuy, isOpen, false)
            {
                Rate = order.price,
                AmountInitial = order.originalAmount,
                Market = market,
                AmountRemaining = order.remainingAmount
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<CoinmateSchema.OrderInfoEntryResponse> GetOrderReponseByIdAsync(RemoteIdContext context, string market)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "currencyPair", market }
            };

            var rRaw = await api.QueryOrdersAsync(body).ConfigureAwait(false);

            var r = rRaw.GetContent();

            var order = r.data.FirstOrDefault(x => x.id.Equals(context.RemoteGroupId));

            if (order == null)
            {
                throw new NoTradeOrderException(context.RemoteGroupId, this);
            }

            return order;
        }

        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                {"coinName", context.Amount.Asset.ShortCode},
                {"amount", context.Amount.ToDecimalValue()},
                {"address", context.Address.Address}
            };

            Response<CoinmateSchema.WithdrawalRequestResponse> rRaw;

            if (context.Amount.Asset.ShortCode.Equals("BTC"))
            {
                rRaw = await api.SubmitWithdrawRequestBitcoinAsync(body).ConfigureAwait(false);
            }
            else if (context.Amount.Asset.ShortCode.Equals("LTC"))
            {
                rRaw = await api.SubmitWithdrawRequestLitecoinAsync(body).ConfigureAwait(false);
            }
            else
            {
                rRaw = await api.SubmitWithdrawRequestBitcoinCashAsync(body).ConfigureAwait(false);
            }

            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();
            
            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.data
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderFeatures = new OrderLimitFeatures(false, CanGetOrderMarket.FromNowhere);
        public OrderLimitFeatures OrderLimitFeatures => OrderFeatures;

        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
    }
}
