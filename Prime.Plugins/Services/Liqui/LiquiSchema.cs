using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Prime.Plugins.Services.Base;

namespace Prime.Plugins.Services.Liqui
{
    public class LiquiSchema : BaseSchema
    {
        #region Base



        #endregion

        #region Private

        //internal class AccountInfoResponse
        //{
        //    public bool success;
        //    public string error;
        //    [JsonProperty("return")]
        //    public AccountInfoDataResponse _return;
        //}

        //internal class AccountInfoDataResponse
        //{
        //    /// <summary>
        //    /// Your account balance available for trading. Doesn’t include funds on your open orders.
        //    /// </summary>
        //    public Dictionary<string, decimal> funds;
        //    /// <summary>
        //    ///  The privileges of the current API key. At this time the privilege to withdraw is not used anywhere.
        //    /// </summary>
        //    public AccountInfoRightsResponse rights;
        //    /// <summary>
        //    /// Deprecated, is equal to 0.
        //    /// </summary>
        //    public int transaction_count;
        //    /// <summary>
        //    /// The number of your open orders.
        //    /// </summary>
        //    public int open_orders;
        //    /// <summary>
        //    /// Server time (UTC).
        //    /// </summary>
        //    public long server_time;
        //}

        //internal class AccountInfoRightsResponse
        //{
        //    public bool info;
        //    public bool trade;
        //    public bool withdraw;
        //}

        #endregion

        #region Public

        public new class TickerData : BaseSchema.TickerData
        {
            public bool status;
        }

        //internal class AssetPairsResponse
        //{
        //    public long server_time;
        //    public Dictionary<string, AssetPairEntry> pairs;
        //}

        //internal class AllTickersResponse : Dictionary<string, TickerResponse>
        //{
        //}

        //internal class OrderBookResponse : Dictionary<string, OrderBookEntryResponse>
        //{
        //    public bool success;
        //}

        //internal class AssetPairEntry
        //{
        //    public int decimal_places;
        //    public decimal min_price;
        //    public decimal max_price;
        //    public decimal min_amount;
        //    public decimal max_amount;
        //    public decimal min_total;
        //    public decimal hidden;
        //    public decimal fee;
        //}

        //internal class TickerResponse
        //{
        //    public decimal high;
        //    public decimal low;
        //    public decimal avg;
        //    public decimal vol;
        //    public decimal vol_cur;
        //    public decimal last;
        //    public decimal buy;
        //    public decimal sell;
        //    public long updated;
        //}

        //internal class OrderBookEntryResponse
        //{
        //    public decimal[][] asks;
        //    public decimal[][] bids;
        //}

        #endregion
    }
}
