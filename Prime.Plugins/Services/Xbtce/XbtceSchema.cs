using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Xbtce
{
    internal class XbtceSchema
    {
        #region Private
        internal class ErrorResponse
        {
            public string Message;
        }

        internal class UserInfoResponse
        {
            public string Id;
            public string Domain;
            public string Group;
            public string AccountingType;
            public string Name;
            public string FirstName;
            public string LastName;
            public string Phone;
            public string Country;
            public string State;
            public string City;
            public string Address;
            public string ZipCode;
            public string SocialSecurityNumber;
            public string Email;
            public string Comment;
            public long Registered;
            public long Modified;
        }

        internal class OrderInfoResponse
        {
            public string Id;
            public string ClientId;
            public int AccountId;
            public string Type;
            public string InitialType;
            public string Side;
            public string Status;
            public string Symbol;
            public decimal StopPrice;
            public decimal Price;
            public decimal CurrentPrice;
            public decimal InitialAmount;
            public decimal RemainingAmount;
            public decimal MaxVisibleAmount;
            public decimal StopLoss;
            public decimal TakeProfit;
            public decimal Margin;
            public decimal Profit;
            public decimal Commission;
            public decimal AgentCommission;
            public decimal Swap;
            public bool ImmediateOrCancel;
            public bool MarketWithSlippage;
            public long Created;
            public long Expired;
            public long Modified;
            public long Filled;
            public long PositionCreated;
            public string Comment;
            public string ClientApp;
        }
        #endregion

        #region Public
        internal class TickerResponse
        {
            public string Symbol;
            public decimal BestBid;
            public decimal BestAsk;
            public decimal LastBuyPrice;
            public decimal LastBuyVolume;
            public decimal LastSellPrice;
            public decimal LastSellVolume;
            public decimal DailyBestBuyPrice;
            public decimal DailyBestSellPrice;
            public decimal DailyTradedBuyVolume;
            public decimal DailyTradedSellVolume;
            public decimal DailyTradedTotalVolume;
            public long LastSellTimestamp;
            public long LastBuyTimestamp;
        }
        #endregion
    }
}
