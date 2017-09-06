using Newtonsoft.Json;
using Rokolab.BitstampClient.Models;
using System;
using System.Collections.Generic;
using Prime.Core;
using RestSharp;
using Prime.Utility;

namespace Rokolab.BitstampClient
{
    public class BitstampClient : IBitstampClient
    {
        private readonly NetworkProviderContext _context;
        private readonly IRequestAuthenticator _requestAuthenticator;
        private const string ApiBase = "https://www.bitstamp.net/api/";
        private Logger L;

        private readonly string _tickerRoute = ApiBase + "ticker/";
        private readonly string _balanceRoute = ApiBase + "v2/balance/";
        private readonly string _transactionsRoute = ApiBase + "v2/user_transactions/btcusd/";
        private readonly string _cancelAllOrdersRoute = ApiBase + "cancel_all_orders/";
        private readonly string _cancelOrderRoute = ApiBase + "v2/cancel_order/";
        private readonly string _openOrdersRoute = ApiBase + "v2/open_orders/btcusd/";
        private readonly string _orderStatusRoute = ApiBase + "order_status/";
        private readonly string _buyRoute = ApiBase + "v2/buy/btcusd/";
        private readonly string _sellRoute = ApiBase + "v2/sell/btcusd/";
        
        private static object _lock = new object();

        private DateTime _lastApiCallTimestamp;

        public BitstampClient(NetworkProviderContext context)
        {
            _context = context;
            L = _context.L;
        }

        public BitstampClient(NetworkProviderContext context, IRequestAuthenticator requestAuthenticator) : this(context)
        {
            _requestAuthenticator = requestAuthenticator;
            _lastApiCallTimestamp = DateTime.Now;
        }

        public TickerResponse GetTicker()
        {
            lock (_lock)
            {
                var request = GetAuthenticatedRequest(Method.POST);
                var response = new RestClient(_tickerRoute).Execute(request);
                _lastApiCallTimestamp = DateTime.Now;
                return JsonConvert.DeserializeObject<TickerResponse>(response.Content);
            }
        }

        public Dictionary<string,string> GetBalance()
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    var response = new RestClient(_balanceRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@GetBalance");
                    return null;
                }
            }
        }

        public WalletAddress GetDepositAddress(IWalletService provider, Asset asset)
        {
            var endp = GetCurrencyEndpoint(asset);
            if (endp == null)
                return null;

            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    var response = new RestClient(ApiBase + endp + "/").Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;
                    if (asset == "BTC".ToAssetRaw())
                        return new WalletAddress(provider, asset)
                        {
                            Address = response.Content.Trim().Trim("\"").Trim()
                        };

                    var d = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

                    var dps = d?.Get("address");
                    if (string.IsNullOrWhiteSpace(dps))
                        return null;

                    var tag = d?.Get("destination_tag", null);

                    if (asset == "XRP".ToAssetRaw() && string.IsNullOrWhiteSpace(tag))
                        return null;
                    
                    return new WalletAddress(provider, asset) {Address = dps.Trim(), Tag = tag};
                }

                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@GetBalance");
                    return null;
                }
            }
        }

        private string GetCurrencyEndpoint(Asset asset)
        {
            switch (asset.ShortCode)
            {
                case "BTC":
                    return "bitcoin_deposit_address";
                case "XRP":
                    return "v2/xrp_address";
                case "LTC":
                    return "v2/ltc_address";
                case "ETH":
                    return "v2/eth_address";
                default:
                    return null;
            }
        }

        public OrderStatusResponse GetOrderStatus(string orderId)
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    request.AddParameter("id", orderId);

                    var response = new RestClient(_orderStatusRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;

                    return JsonConvert.DeserializeObject<OrderStatusResponse>(response.Content);
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@GetOrderStatus");
                    return null;
                }
            }
        }

        public List<OpenOrderResponse> GetOpenOrders()
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    var response = new RestClient(_openOrdersRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;
                    return JsonConvert.DeserializeObject<List<OpenOrderResponse>>(response.Content);
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@GetOpenOrders");
                    return null;
                }
            }
        }

        public List<TransactionResponse> GetTransactions(int offset, int limit)
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    request.AddParameter("offset", offset);
                    request.AddParameter("limit", limit);

                    var response = new RestClient(_transactionsRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;
                    return JsonConvert.DeserializeObject<List<TransactionResponse>>(response.Content);
                }
                catch (Exception ex)
                {
                   L.Error(ex, "BitstampClient@GetTransactions");
                    return null;
                }
            }
        }

        public bool CancelOrder(string orderId)
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    request.AddParameter("id", orderId);

                    var response = new RestClient(_cancelOrderRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;

                    return (response.Content == "true") ? true : false;
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@CancelOrder");
                    return false;
                }
            }
        }

        public bool CancelAllOrders()
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);

                    var response = new RestClient(_cancelAllOrdersRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;

                    return (response.Content == "true") ? true : false;
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@CancelAllOrders");
                    return false;
                }
            }
        }

        public BuySellResponse Buy(double amount, double price)
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    request.AddParameter("amount", amount.ToString().Replace(",", "."));
                    request.AddParameter("price", price.ToString().Replace(",", "."));

                    var response = new RestClient(_buyRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;
                    return JsonConvert.DeserializeObject<BuySellResponse>(response.Content);
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@Buy");
                    return null;
                }
            }
        }

        public BuySellResponse Sell(double amount, double price)
        {
            lock (_lock)
            {
                try
                {
                    var request = GetAuthenticatedRequest(Method.POST);
                    request.AddParameter("amount", amount.ToString().Replace(",", "."));
                    request.AddParameter("price", price.ToString().Replace(",", "."));

                    var response = new RestClient(_sellRoute).Execute(request);
                    _lastApiCallTimestamp = DateTime.Now;
                    return JsonConvert.DeserializeObject<BuySellResponse>(response.Content);
                }
                catch (Exception ex)
                {
                    L.Error(ex, "BitstampClient@Sell");
                    return null;
                }
            }
        }

        private RestRequest GetAuthenticatedRequest(Method method)
        {
            var request = new RestRequest(method);
            _requestAuthenticator.Authenticate(request);
            return request;
        }
    }
}