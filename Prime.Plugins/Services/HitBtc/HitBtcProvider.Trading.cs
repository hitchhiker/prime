﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.HitBtc
{
    public partial class HitBtcProvider : IBalanceProvider, IDepositProvider, IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if(r.ResponseMessage.IsSuccessStatusCode) return;

            var context = r.GetContent();

            if (context is HitBtcSchema.BaseResponse rError && rError.error != null)
            {
                ThrowHitBtcErrorException(rError.error);
            }
            else
            {
                var innerError = context.GetMemberValue<HitBtcSchema.ErrorResponse>("error");
                if (innerError != null)
                    ThrowHitBtcErrorException(innerError);
            }
        }

        private void ThrowHitBtcErrorException(HitBtcSchema.ErrorResponse error, [CallerMemberName] string method = "Unknown")
        {
            throw new ApiResponseException($"{error.message.Trim(".")} ({error.code}){ (string.IsNullOrWhiteSpace(error.description) ? "" : $": { error.description.Trim(".") }") }", this, method);
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetBalancesAsync().ConfigureAwait(false);
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
            body.Add("quantity", context.Quantity);
            body.Add("price", context.Rate.ToDecimalValue());
            body.Add("type", "limit");
            body.Add("timeInForce", "GTC");
            body.Add("strictValidate", "false");

            var rRaw = await api.CreateNewOrderAsync(body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.clientOrderId);
        }

        private Dictionary<string, object> CreateHitBtcRequestBody()
        {
            return new Dictionary<string, object>();
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rRaw = await api.GetActiveOrderInfoAsync(context.RemoteGroupId).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var isOpen = r.status.Equals("new", StringComparison.OrdinalIgnoreCase);
            var isCancelRequested = r.status.Equals("new", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatus(r.clientOrderId, isOpen, isCancelRequested);
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();
    }
}
