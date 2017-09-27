using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.BitMex
{
    internal class BitMexSchema
    {
        internal class DepositAddress
        {
            public string address;
        }

        internal class BucketedTradeEntriesResponse : List<BucketedTradeEntryResponse>
        {
            
        }

        internal class BucketedTradeEntryResponse
        {
            public DateTime timestamp;
            public string symbol;
            public decimal open;
            public decimal high;
            public decimal low;
            public decimal close;
            public decimal trades;
            public decimal volume;
            public decimal vwap;
            public decimal lastSize;
            public decimal turnover;
            public decimal homeNotional;
            public decimal foreignNotional;
        }

        internal class WalletInfo
        {
            public int account;
            public string currency;
            public int prevDeposited;
            public int prevWithdrawn;
            public int prevTransferIn;
            public int prevTransferOut;
            public int prevAmount;
            public DateTime prevTimestamp;
            public int deltaDeposited;
            public int deltaWithdrawn;
            public int deltaTransferIn;
            public int deltaTransferOut;
            public int deltaAmount;
            public int deposited;
            public int withdrawn;
            public int transferIn;
            public int transferOut;
            public int amount;
            public int pendingCredit;
            public int pendingDebit;
            public int confirmedDebit;
            public DateTime timestamp;
            public string addr;
            public string script;
            public List<string> withdrawalLock;
        }

        internal class UserInfo
        {
            public int id { get; set; }
            public int? ownerId { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public DateTime created { get; set; }
            public DateTime lastUpdated { get; set; }
            public UserPreferences preferences { get; set; }
            public string TFAEnabled { get; set; }
            public string affiliateID { get; set; }
            public string pgpPubKey { get; set; }
            public string country { get; set; }
        }

        internal class UserPreferences
        {
            public bool animationsEnabled { get; set; }
            public DateTime announcementsLastSeen { get; set; }
            public int chatChannelID { get; set; }
            public string colorTheme { get; set; }
            public string currency { get; set; }
            public bool debug { get; set; }
            public List<string> disableEmails { get; set; }
            public List<string> hideConfirmDialogs { get; set; }
            public bool hideConnectionModal { get; set; }
            public bool hideFromLeaderboard { get; set; }
            public bool hideNameFromLeaderboard { get; set; }
            public List<string> hideNotifications { get; set; }
            public string locale { get; set; }
            public List<string> msgsSeen { get; set; }
            public OrderBookBinning orderBookBinning { get; set; }
            public string orderBookType { get; set; }
            public bool orderClearImmediate { get; set; }
            public bool orderControlsPlusMinus { get; set; }
            public List<string> sounds { get; set; }
            public bool strictIPCheck { get; set; }
            public bool strictTimeout { get; set; }
            public string tickerGroup { get; set; }
            public bool tickerPinned { get; set; }
            public string tradeLayout { get; set; }
        }

        internal class OrderBookBinning
        {
            
        }

        internal class InstrumentsActiveResponse : List<InstrumentResponse>
        {
        }

        internal class InstrumentsResponse : List<InstrumentResponse>
        {
            
        }

        internal class InstrumentLatestPricesResponse : List<InstrumentLatestPriceResponse>
        {
            
        }

        internal class InstrumentLatestPriceResponse
        {
            public string symbol;
            public DateTime timestamp;
            public decimal? lastPrice;
            public string underlying;
            public string quoteCurrency;
        }

        internal class InstrumentResponse
        {
            public string symbol;
            public string rootSymbol;
            public string state;
            public string typ;
            public string listing;
            public string front;
            public string expiry;
            public string settle;
            public string relistInterval;
            public string inverseLeg;
            public string sellLeg;
            public string buyLeg;
            public string positionCurrency;
            public string underlying;
            public string quoteCurrency;
            public string underlyingSymbol;
            public string reference;
            public string referenceSymbol;
            public string calcInterval;
            public string publishInterval;
            public string publishTime;
            public string maxOrderQty;
            public string maxPrice;
            public string lotSize;
            public string tickSize;
            public string multiplier;
            public string settlCurrency;
            public string underlyingToPositionMultiplier;
            public string underlyingToSettleMultiplier;
            public string quoteToSettleMultiplier;
            public string isQuanto;
            public string isInverse;
            public string initMargin;
            public string maintMargin;
            public string riskLimit;
            public string riskStep;
            public string limit;
            public string capped;
            public string taxed;
            public string deleverage;
            public string makerFee;
            public string takerFee;
            public string settlementFee;
            public string insuranceFee;
            public string fundingBaseSymbol;
            public string fundingQuoteSymbol;
            public string fundingPremiumSymbol;
            public string fundingTimestamp;
            public string fundingInterval;
            public string fundingRate;
            public string indicativeFundingRate;
            public string rebalanceTimestamp;
            public string rebalanceInterval;
            public string openingTimestamp;
            public string closingTimestamp;
            public string sessionInterval;
            public string prevClosePrice;
            public string limitDownPrice;
            public string limitUpPrice;
            public string bankruptLimitDownPrice;
            public string bankruptLimitUpPrice;
            public string prevTotalVolume;
            public string totalVolume;
            public double volume;
            public string volume24h;
            public string prevTotalTurnover;
            public string totalTurnover;
            public string turnover;
            public string turnover24h;
            public string prevPrice24h;
            public double vwap;
            public double highPrice;
            public double lowPrice;
            public decimal lastPrice;
            public string lastPriceProtected;
            public string lastTickDirection;
            public string lastChangePcnt;
            public string bidPrice;
            public string midPrice;
            public string askPrice;
            public string impactBidPrice;
            public string impactMidPrice;
            public string impactAskPrice;
            public string hasLiquidity;
            public string openInterest;
            public double openValue;
            public string fairMethod;
            public string fairBasisRate;
            public string fairBasis;
            public string fairPrice;
            public string markMethod;
            public string markPrice;
            public string indicativeTaxRate;
            public string indicativeSettlePrice;
            public string settledPrice;
            public DateTime timestamp;
        }
    }
}
