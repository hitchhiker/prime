using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using RestEase;

namespace Prime.Plugins.Services.Coinbase
{
    public partial class CoinbaseProvider : IBalanceProvider, IDepositProvider, IOrderLimitProvider
    {
        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountsAsync().ConfigureAwait(false);

            var results = new BalanceResults(this);

            foreach (var a in r.data)
            {
                if (a.balance == null)
                    continue;

                var c = a.balance.currency.ToAsset(this);
                results.Add(c, a.balance.amount, 0);
            }

            return results;
        }

        public async Task<WalletAddressesResult> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var accid = "";

            var accs = await api.GetAccountsAsync().ConfigureAwait(false);
            var ast = context.Asset.ToRemoteCode(this);

            var acc = accs.data.FirstOrDefault(x => string.Equals(x.currency, ast, StringComparison.OrdinalIgnoreCase));
            if (acc == null)
                return null;

            accid = acc.id;

            if (accid == null)
                return null;

            var r = await api.GetAddressesAsync(acc.id).ConfigureAwait(false);

            //if (r.data.Count == 0 && context.CanGenerateAddress)
            //{
            //    var cr = await api.CreateAddressAsync(accid);
            //    if (cr != null)
            //        r.data.AddRange(cr.data);
            //}

            var addresses = new WalletAddressesResult();

            foreach (var a in r.data)
            {
                if (string.IsNullOrWhiteSpace(a.address))
                    continue;

                var forasset = FromNetwork(a.network);
                if (!context.Asset.Equals(forasset))
                    continue;

                addresses.Add(new WalletAddress(this, context.Asset) { Address = a.address });
            }

            return addresses;
        }

        public async Task<WalletAddressesResult> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var accs = await api.GetAccountsAsync().ConfigureAwait(false);
            var addresses = new WalletAddressesResult();

            var accountIds = accs.data.Select(x => new KeyValuePair<string, string>(x.currency, x.id));

            foreach (var kvp in accountIds)
            {
                var r = await api.GetAddressesAsync(kvp.Value).ConfigureAwait(false);

                foreach (var rAddress in r.data)
                {
                    if (string.IsNullOrWhiteSpace(rAddress.address))
                        continue;

                    addresses.Add(new WalletAddress(this, kvp.Key.ToAsset(this))
                    {
                        Address = rAddress.address
                    });
                }
            }

            return addresses;
        }

        private void CheckResponseErrors<T>(Response<T> response, [CallerMemberName] string method = "Unknown")
        {
            if (!response.ResponseMessage.IsSuccessStatusCode && response.TryGetContent(out CoinbaseSchema.ErrorResponse rError))
            {
                if(rError.errors.Count > 0)
                    throw new ApiResponseException($"API error: {rError.errors[0].id} ({rError.errors[0].message})", this, method);

                throw new ApiResponseException($"API response error occurred - {response.ResponseMessage.ReasonPhrase} ({response.ResponseMessage.StatusCode})", this, method);
            }
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);
            
            // Get payment method.
            var rPaymentMethodsRaw = await api.GetPaymentMethodsAsync().ConfigureAwait(false);
            CheckResponseErrors(rPaymentMethodsRaw);

            var rPaymentMethods = rPaymentMethodsRaw.GetContent();

            if(rPaymentMethods.data.Count == 0)
                throw new ApiResponseException("No payment methods found in account", this);

            // TODO: AY: HH, we don't support payment methods selection. Maybe we need to consider it in Prime architecture. Just selecting first payment method at the moment.
            var paymentMethodId = rPaymentMethods.data.FirstOrDefault()?.id;

            var body = new Dictionary<string, object>()
            {
                {"amount", context.Quantity.ToDecimalValue() }, // Buy amount without fees.
                {"currency", context.Pair.Asset1 },
                {"payment_method", paymentMethodId }
            };

            // Get account number.
            var accounts = await api.GetAccountsAsync().ConfigureAwait(false);
            var asset = context.Pair.Asset1.ToRemoteCode(this);

            var account = accounts.data.FirstOrDefault(x => string.Equals(x.currency, asset, StringComparison.OrdinalIgnoreCase));
            if (account == null)
                throw new ApiResponseException("No account found to proceed payment on specified market", this);

            var accountId = account.id;

            // Call endpoint.
            var rRaw = context.IsBuy 
                ? await api.PlaceBuyOrderAsync(accountId, body).ConfigureAwait(false)
                : await api.PlaceSellOrderAsync(accountId, body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.data.id);
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

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderLimitFeaturesStatic = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderLimitFeaturesStatic;
    }
}
