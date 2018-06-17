using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Gate
{
    internal class GateSchema
    {
        #region Public

        internal class VolumeResponse
        {
            public bool result;
            public VolumeEntry[] data;
        }

        internal class TickerResponse
        {
            public bool result;
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public decimal high24hr;
            public decimal low24hr;
        }

        internal class VolumeEntry
        {
            public int no;
            public decimal rate;
            public decimal vol_a;
            public decimal vol_b;
            public decimal rate_percent;
            public decimal supply;
            public string symbol;
            public string name;
            public string name_en;
            public string name_cn;
            public string pair;
            public string marketcap;
            public string plot;
            public string curr_a;
            public string curr_b;
            public string curr_suffix;
            public string trend;
        }

        internal class OrderBookResponse
        {
            public bool result;
            public decimal[][] bids;
            public decimal[][] asks;
        }

        #endregion

        #region Private

        internal class ErrorResponse
        {
            public bool result;
            public string code;
            public string message;
        }

        internal class BalancesResponse
        {
            public bool result;
            public object[] available;
        }

        internal class NewOrderResponse
        {
            public bool result;
            public string orderNumber;
            public decimal rate;
            public decimal leftAmount;
            public decimal filledAmount;
            public decimal filledRate;
            public string msg;
        }

        internal class OrderResponse
        {
            public bool result;
            public string msg;
            public OrderEntryResponse order;
        }

        internal class OrderEntryResponse
        {
            public string id;
            public string status;
            public string pair;
            public string type;
            public decimal rate;
            public decimal amount;
            public decimal initial_rate;
            public decimal initial_amount;
        }

        internal class WithdrawalResponse
        {
            public bool success;
            public string messa;
        }

        #endregion
    }
}
