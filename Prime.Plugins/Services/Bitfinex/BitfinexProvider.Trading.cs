using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bitfinex
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://bitfinex.readme.io/v1/reference
    public partial class BitfinexProvider : IOrderLimitProvider, IBalanceProvider
    {
        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new BitfinexSchema.NewOrderRequest.Descriptor();
            body.symbol = context.Pair.ToTicker(this);
            body.amount = context.Quantity.ToString(CultureInfo.InvariantCulture);
            body.price = context.Rate.ToDecimalValue().ToString(CultureInfo.InvariantCulture);
            body.side = context.IsSell ? "sell" : "buy";

            var rRaw = await api.PlaceNewOrderAsync(body).ConfigureAwait(false);

            CheckBitfinexResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.order_id.ToString());
        }

        public Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            throw new NotImplementedException();
        }

        public decimal MinimumTradeVolume { get; }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new BitfinexSchema.WalletBalancesRequest.Descriptor();

            var rRaw = await api.GetWalletBalancesAsync(body).ConfigureAwait(false);

            CheckBitfinexResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var balances = new BalanceResults();

            foreach (var rBalance in r)
            {
                var asset = rBalance.currency.ToAsset(this);

                balances.Add(new BalanceResult(this)
                {
                    Available = new Money(rBalance.available, asset),
                    AvailableAndReserved = new Money(rBalance.amount, asset),
                    Reserved = new Money(0, asset)
                });
            }

            return balances;
        }
    }
}
