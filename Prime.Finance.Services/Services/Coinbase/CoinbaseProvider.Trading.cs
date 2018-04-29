using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Core.Api.Request.Response;
using RestEase;

namespace Prime.Finance.Services.Services.Coinbase
{
    // https://developers.coinbase.com/api/v2
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public partial class CoinbaseProvider : IBalanceProvider, IDepositProvider, IOrderLimitProvider
    {
        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAccountsAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

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

            var rAccountId = await GetFirstAccountId(context).ConfigureAwait(false);

            var r = await api.GetAddressesAsync(rAccountId.Result).ConfigureAwait(false);

            //if (r.data.Count == 0 && context.CanGenerateAddress)
            //{
            //    var cr = await api.CreateAddressAsync(accid);
            //    if (cr != null)
            //        r.data.AddRange(cr.data);
            //}

            var addresses = new WalletAddressesResult()
            {
                ApiHitsCount = rAccountId.ApiHitsCount + 1
            };

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
            var accsRaw = await api.GetAccountsAsync().ConfigureAwait(false);
            CheckResponseErrors(accsRaw);

            var accs = accsRaw.GetContent();

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
                if (rError.errors.Count > 0)
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

            if (rPaymentMethods.data.Count == 0)
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
            var asset = context.Pair.Asset1.ToRemoteCode(this);
            var rAccountId = await GetFirstAccountId(context,
                x => string.Equals(x.currency, asset, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);

            var accountId = rAccountId.Result;

            // Call endpoint.
            var rRaw = context.IsBuy
                ? await api.PlaceBuyOrderAsync(accountId, body).ConfigureAwait(false)
                : await api.PlaceSellOrderAsync(accountId, body).ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.data.id)
            {
                ApiHitsCount = rAccountId.ApiHitsCount + 2
            };
        }

        public async Task<TradeOrdersResponse> GetOrdersHistory(TradeOrdersContext context)
        {
            var rAllOrders = await GetBuysAndSellsAsync(context).ConfigureAwait(false);

            var orders = rAllOrders.Result.Where(x => !x.status.Equals("created", StringComparison.OrdinalIgnoreCase)).Select(x =>
            {
                var isBuy = x.resource.Equals("buy", StringComparison.OrdinalIgnoreCase);
                var isCanceled = x.status.Equals("canceled", StringComparison.OrdinalIgnoreCase);
                return new TradeOrderStatus(Network, x.id, isBuy, false, isCanceled)
                {
                    AmountInitial = x.amount.amount,
                    Market = new AssetPair(x.amount.currency, x.total.currency, this),
                    Rate = x.subtotal.amount / x.amount.amount
                };
            });

            return new TradeOrdersResponse(orders)
            {
                ApiHitsCount = rAllOrders.ApiHitsCount
            };
        }

        /// <summary>
        /// Returns merged list of buy and sell orders.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<ResponseModelBase<IEnumerable<CoinbaseSchema.OrderResponse>>> GetBuysAndSellsAsync(MarketOrdersContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rAccountId = await GetFirstAccountId(context).ConfigureAwait(false);
            var accountId = rAccountId.Result;

            var rBuysRaw = await api.ListBuysAsync(accountId).ConfigureAwait(false);
            CheckResponseErrors(rBuysRaw);

            var rSellsRaw = await api.ListSellsAsync(accountId).ConfigureAwait(false);
            CheckResponseErrors(rSellsRaw);

            var rBuys = rBuysRaw.GetContent();
            var rSells = rSellsRaw.GetContent();

            return new ResponseModelBase<IEnumerable<CoinbaseSchema.OrderResponse>>(rBuys.data.Concat(rSells.data))
            {
                ApiHitsCount = rAccountId.ApiHitsCount + 2
            };
        }

        public async Task<OpenOrdersResponse> GetOpenOrdersAsync(OpenOrdersContext context)
        {
            var rAllOrders = await GetBuysAndSellsAsync(context).ConfigureAwait(false);
            var allOrders = rAllOrders.Result;

            var orders = new List<TradeOrderStatus>();

            foreach (var rOpenOrder in allOrders.Where(x => x.status.Equals("created", StringComparison.OrdinalIgnoreCase)))
            {
                var isBuy = rOpenOrder.resource.Equals("buy", StringComparison.OrdinalIgnoreCase);

                orders.Add(new TradeOrderStatus(Network, rOpenOrder.id, isBuy, true, false)
                {
                    AmountInitial = rOpenOrder.amount.amount,
                    Market = new AssetPair(rOpenOrder.amount.currency, rOpenOrder.total.currency, this),
                    Rate = rOpenOrder.subtotal.amount / rOpenOrder.amount.amount
                });
            }

            return new OpenOrdersResponse(orders)
            {
                ApiHitsCount = rAllOrders.ApiHitsCount
            };
        }

        /// <summary>
        /// Returns ID of the first available account. API hits: 1.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        private async Task<ResponseModelBase<string>> GetFirstAccountId(NetworkProviderPrivateContext context, Func<CoinbaseSchema.AccountResponse, bool> by = null)
        {
            var api = ApiProvider.GetApi(context);

            var accountsRaw = await api.GetAccountsAsync().ConfigureAwait(false);
            CheckResponseErrors(accountsRaw);
            var accounts = accountsRaw.GetContent();

            var account = by == null ? accounts.data.FirstOrDefault() : accounts.data.FirstOrDefault(by);
            if (account == null)
                throw new ApiResponseException("No account found to get list of open orders", this);

            return new ResponseModelBase<string>(account.id)
            {
                ApiHitsCount = 1
            };
        }

        public async Task<TradeOrderStatusResponse> GetOrderStatusAsync(RemoteMarketIdContext context)
        {
            var api = ApiProvider.GetApi(context);

            var rAccountId = await GetFirstAccountId(context).ConfigureAwait(false);
            var accountId = rAccountId.Result;

            var apiHits = rAccountId.ApiHitsCount;

            var rBuyRaw = await api.ShowBuyOrder(accountId, context.RemoteGroupId).ConfigureAwait(false);

            CoinbaseSchema.ShowOrderResponse order = null;
            apiHits++;

            if (rBuyRaw.ResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                var rSellRaw = await api.ShowSellOrder(accountId, context.RemoteGroupId).ConfigureAwait(false);
                apiHits++;

                CheckResponseErrors(rSellRaw);

                order = rSellRaw.GetContent();
            }
            else if (rBuyRaw.ResponseMessage.IsSuccessStatusCode)
            {
                CheckResponseErrors(rBuyRaw);

                order = rBuyRaw.GetContent();
            }

            if (order == null)
                throw new NoTradeOrderException(context, this);

            var isBuy = order.data.resource.Equals("buy", StringComparison.OrdinalIgnoreCase);
            var isOpen = order.data.status.Equals("created", StringComparison.OrdinalIgnoreCase);

            return new TradeOrderStatusResponse(Network, order.data.id, isBuy, isOpen, false)
            {
                TradeOrderStatus =
                {
                    AmountInitial = order.data.amount.amount,
                    Market = new AssetPair(order.data.amount.currency, order.data.total.currency, this),
                    Rate = order.data.subtotal.amount / order.data.amount.amount
                },
                ApiHitsCount = apiHits
            };
        }

        public Task<OrderMarketResponse> GetMarketFromOrderAsync(RemoteIdContext context) => Task.FromResult<OrderMarketResponse>(null);

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();

        private static readonly OrderLimitFeatures OrderLimitFeaturesStatic = new OrderLimitFeatures(false, CanGetOrderMarket.WithinOrderStatus);
        public OrderLimitFeatures OrderLimitFeatures => OrderLimitFeaturesStatic;
    }
}
