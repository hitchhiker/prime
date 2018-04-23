using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using plugins;
using RestEase;

namespace CryptoFacilities.Api.V1
{
    public class CryptoFacilitiesApi
    {
        public static List<HistoryLine> Test(Action<string> consoleWrite = null)
        {
            // Create an implementation of that interface
            // We'll pass in the base URL for the API
            var api = RestClient.For<ICryptoFacilitiesApi>("https://www.cryptofacilities.com/derivatives/api/v3/");

            var response = api.GetHistoryAsync("F-XBT:USD-Mar18").Result;
            var s = response.GetContent();
            var r = s.Items.OrderBy(x => x.UtcDate).ToList();
            foreach (var historyLine in r)
            {
                consoleWrite?.Invoke($"Date: {historyLine.UtcDate}");
            }
            return r;
        }

        private const string endpoint = "https://www.cryptofacilities.com/api/v3/history";
        private const string _endpoint = "https://www.cryptofacilities.com/derivatives";

        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly int rateLimit;

        /// <summary>
        /// Contruct the API interface object.
        ///
        /// This class is thread safe.
        /// </summary>
        /// <param name="key">API Key</param>
        /// <param name="secret">API Secret Key</param>
        /// <param name="rateLimit">Rate limit in milliseconds</param>
        public CryptoFacilitiesApi(string key, string secret, int rateLimit = 500)
        {
            this.apiKey = key;
            this.apiSecret = secret;
            this.rateLimit = rateLimit;
        }

        /// <summary>
        /// Execute a query to the CryptoFacilities API.
        /// </summary>
        /// <param name="path">API Path (e.g. /api/ticker)</param>
        /// <param name="param">Parameters and their values</param>
        /// <param name="auth">Set to true when authentication is to be used</param>
        /// <returns></returns>
        public string Query(string path, Dictionary<string, string> param = null, bool auth = false)
        {
            RateLimit();

            string postData = BuildPostData(param);

            string url = endpoint + path;
            if (postData != "")
                url += "?" + postData;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";

            if (auth)
                AddHeaders(webRequest, path, postData);

            if (postData != "")
            {
                using (var writer = new StreamWriter(webRequest.GetRequestStreamAsync().Result))
                {
                    writer.Write(postData);
                }
            }

            using (WebResponse webResponse = webRequest.GetResponseAsync().Result)
            using (Stream str = webResponse.GetResponseStream())
            using (StreamReader sr = new StreamReader(str))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// This method returns all contracts currently listed, together with their specifications.
        /// </summary>
        /// <returns></returns>
        public List<Contract> GetContracts()
        {
            string res = Query("/api/contracts");
            var response = JsonConvert.DeserializeObject<GetContractsResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Contracts;
        }

        /// <summary>
        /// This method returns the current best bid and ask prices for a contract (i.e. Level 1 market depth).
        /// </summary>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15)</param>
        /// <param name="unit">The currency of denomination of the contract (always USD)</param>
        /// <param name="ask">Current best ask price</param>
        /// <param name="bid">Current best bid price</param>
        public void GetTicker(string tradeable, string unit, out decimal ask, out decimal bid)
        {
            var param = new Dictionary<string, string>();
            param["tradeable"] = tradeable;
            param["unit"] = unit;

            string res = Query("/api/ticker", param);
            var response = JsonConvert.DeserializeObject<GetTickerResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);

            ask = response.Ask;
            bid = response.Bid;
        }

        /// <summary>
        /// This method returns the current best bid and best ask prices for a contract (i.e. Level 2 market depth), together with their cumulative volumes.
        /// </summary>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15)</param>
        /// <param name="unit">The currency of denomination of the contract (always USD)</param>
        /// <param name="cumulatedBids">Array or the form [[bid, cumQty],[bid, cumQty],...,[bid, cumQty]]</param>
        /// <param name="cumulatedAsks">Array or the form [[ask, cumQty],[ask, cumQty],...,[ask, cumQty]]</param>
        public void GetCumulativeBidAsk(
            string tradeable,
            string unit,
            out decimal[][] cumulatedBids,
            out decimal[][] cumulatedAsks)
        {
            var param = new Dictionary<string, string>();
            param["tradeable"] = tradeable;
            param["unit"] = unit;

            string res = Query("/api/cumulativebidask", param);
            var response = JsonConvert.DeserializeObject<GetCumulativeBidAskResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);

            var cBids = JsonConvert.DeserializeObject<decimal[][]>(response.CumulatedBids);
            var cAsks = JsonConvert.DeserializeObject<decimal[][]>(response.CumulatedAsks);

            cumulatedBids = cBids.OrderByDescending(x => x[0]).ToArray();
            cumulatedAsks = cAsks;
        }

        /// <summary>
        /// This method returns the current price of the CF-BPI.
        /// </summary>
        /// <returns></returns>
        public decimal GetCFBPI()
        {
            string res = Query("/api/cfbpi");
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            return Convert.ToDecimal(response["cf-bpi"], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method returns an estimate of the current annualized volatility of the CF-BPI.
        /// It is calculated as the standard deviation of log returns of the last 60 observed
        /// minutely prices of the CF-BPI, scaled by sqrt( 60 * 24 * 365 ). It is updated every
        /// 60 seconds and is provided purely for informational purposes but you might find it
        /// useful for monitoring the risk of your portfolio.
        /// </summary>
        /// <returns></returns>
        public decimal GetVolatility()
        {
            string res = Query("/api/volatility");
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            return Convert.ToDecimal(response["volatility"], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method returns your bitcoin account balance, together with the balances of
        /// all contract positions you might have.
        /// </summary>
        /// <returns>Dictionary with (Tradeable, Balance) entries</returns>
        public Dictionary<string, decimal> GetBalance()
        {
            string res = Query("/api/balance", auth: true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            var result = new Dictionary<string, decimal>();

            foreach (var row in response)
            {
                if (row.Key == "result")
                    continue;
                result.Add(row.Key, (decimal)double.Parse(row.Value.ToString(), CultureInfo.InvariantCulture));
            }

            return result;
        }

        /// <summary>
        /// This method allows you to place an order to buy or sell contracts.
        /// </summary>
        /// <param name="type">The order type (always LMT )</param>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15 )</param>
        /// <param name="unit">The currency of denomination of the contract (always USD )</param>
        /// <param name="dir">The direction of the order, either Buy or Sell </param>
        /// <param name="qty">The quantity of the order. This must be an integer number</param>
        /// <param name="price">The limit buy or sell price. This must be a multiple of the tick size, which is 0.01</param>
        /// <returns></returns>
        public string PlaceOrder(
            string type,
            string tradeable,
            string unit,
            string dir,
            int qty,
            decimal price)
        {
            var param = new Dictionary<string, string>();
            param["type"] = "LMT";
            param["tradeable"] = tradeable;
            param["unit"] = unit;
            param["dir"] = dir;
            param["qty"] = qty.ToString();
            param["price"] = Convert.ToString(price, CultureInfo.InvariantCulture);

            string res = Query("/api/placeOrder", param, true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            return response["orderId"].ToString();
        }

        /// <summary>
        /// This method allows you to place an order to buy or sell contracts.
        /// The method sets the Uid field of the order.
        /// </summary>
        /// <param name="order">The order to palce</param>
        /// <returns>The order id</returns>
        public string PlaceOrder(ref Order order)
        {
            return order.Uid = PlaceOrder(order.Type, order.Tradeable, order.Unit, order.Dir, order.Qty, order.Price);
        }

        /// <summary>
        /// This method allows you to cancel an order that you submitted previously and that
        /// has not been matched yet.
        /// </summary>
        /// <param name="uid">The identifier of the order that you want to cancel. This identifier is returned by the methods 'Place Order' or 'Open Orders'</param>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15)</param>
        /// <param name="unit">The currency of denomination of the contract (always USD)</param>
        /// <returns>Returns true on success</returns>
        public bool CancelOrder(string uid, string tradeable, string unit)
        {
            var param = new Dictionary<string, string>();
            param["uid"] = uid;
            param["tradeable"] = tradeable;
            param["unit"] = unit;

            string res = Query("/api/cancelOrder", param, true);
            var response = (JObject)JsonConvert.DeserializeObject(res);

            return response["result"].ToString() == "success";
        }

        /// <summary>
        /// This method allows you to cancel an order that you submitted previously and that
        /// has not been matched yet.
        /// </summary>
        /// <param name="order">The order to cancel</param>
        /// <returns>Returns true on success</returns>
        public bool CancelOrder(Order order)
        {
            return CancelOrder(order.Uid, order.Tradeable, order.Unit);
        }

        /// <summary>
        /// This method allows you to retrieve information on all of your open orders.
        /// </summary>
        /// <returns>A list of OrderInfo objects</returns>
        public List<OrderInfo> GetOpenOrders()
        {
            string res = Query("/api/openOrders", auth: true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            try { return JsonConvert.DeserializeObject<List<OrderInfo>>(response["orders"].ToString()); }
            catch (Exception) { return new List<OrderInfo>(); }
        }

        /// <summary>
        /// This method allows you to retrieve information on your last matched orders.
        /// </summary>
        /// <param name="number"> the number of matched orders to return. This must be an integer number. The method's return is capped to a maximum of 100 matched orders</param>
        /// <returns>A list of TradeInfo objects</returns>
        public List<TradeInfo> GetTrades(int number = 100)
        {
            var param = new Dictionary<string, string>();
            param["number"] = number.ToString();

            string res = Query("/api/trades", param, auth: true);
            var response = JsonConvert.DeserializeObject<GetTradesResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);

            return response.Trades ?? new List<TradeInfo>();
        }

        #region Utility methods

        private void AddHeaders(HttpWebRequest webRequest, string path, string postData)
        {
            string nonce = GetNonce().ToString();

            byte[] h = sha256_hash(postData + nonce + path);
            byte[] base64DecodedSecret = Convert.FromBase64String(apiSecret);
            byte[] r = hmacsha512(base64DecodedSecret, h);
            string authent = Convert.ToBase64String(r);

            webRequest.Headers["APIKey"] =apiKey;
            webRequest.Headers["Nonce"] = nonce;
            webRequest.Headers["Authent"] = authent;
        }

        private string BuildPostData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, item.Value));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        private long GetNonce()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private byte[] sha256_hash(string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));
                return result;
            }
        }

        private byte[] hmacsha512(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA512(keyByte))
            {
                byte[] result = hash.ComputeHash(messageBytes);
                return result;
            }
        }

        #endregion Utility methods

        #region RateLimiter

        private long lastTicks = 0;
        private object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        #endregion RateLimiter
    }
}