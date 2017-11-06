using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.BitMex
{
    internal class BitMexSchema
    {
        internal class BucketedTradeEntriesResponse : List<BucketedTradeEntryResponse> { }

        internal class OrderBookResponse : List<OrderBookRecordResponse> { }

        internal class WalletHistoryResponse : List<WalletHistoryEntryResponse> { }

        internal class RequestWithdrawalResponse
        {
            public string transactID;
            public int account;
            public string currency;
            public string transactType;
            public decimal amount;
            public decimal fee;
            public string transactStatus;
            public string address;
            public string tx;
            public string text;
            public DateTime transactTime;
            public DateTime timestamp;
        }

        internal class WalletHistoryEntryResponse
        {
            public string transactID;
            public int account;
            public string currency;
            public string transactType;
            public decimal amount;
            public decimal? fee;
            public string transactStatus;
            public string address;
            public DateTime transactTime;
            public decimal walletBalance;
            public decimal? marginBalance;
            public DateTime timestamp;
        }

        internal class OrderBookRecordResponse
        {
            public string symbol;
            public long id;
            public string side;
            public decimal size;
            public decimal price;
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
            public decimal? vwap;
            public decimal? lastSize;
            public decimal turnover;
            public decimal homeNotional;
            public decimal foreignNotional;
        }

        internal class WalletInfoResponse
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

        internal class UserInfoResponse
        {
            public int id;
            public int? ownerId;
            public string firstname;
            public string lastname;
            public string username;
            public string email;
            public string phone;
            public DateTime created;
            public DateTime lastUpdated;
            public UserPreferences preferences;
            public string TFAEnabled;
            public string affiliateID;
            public string pgpPubKey;
            public string country;
        }

        internal class UserPreferences
        {
            public bool animationsEnabled;
            public DateTime announcementsLastSeen;
            public int chatChannelID;
            public string colorTheme;
            public string currency;
            public bool debug;
            public List<string> disableEmails;
            public List<string> hideConfirmDialogs;
            public bool hideConnectionModal;
            public bool hideFromLeaderboard;
            public bool hideNameFromLeaderboard;
            public List<string> hideNotifications;
            public string locale;
            public List<string> msgsSeen;
            public OrderBookBinning orderBookBinning;
            public string orderBookType;
            public bool orderClearImmediate;
            public bool orderControlsPlusMinus;
            public List<string> sounds;
            public bool strictIPCheck;
            public bool strictTimeout;
            public string tickerGroup;
            public bool tickerPinned;
            public string tradeLayout;
        }

        internal class OrderBookBinning { }

        internal class InstrumentsActiveResponse : List<InstrumentResponse> { }

        internal class InstrumentsResponse : List<InstrumentResponse> { }

        internal class InstrumentLatestPricesResponse : List<InstrumentLatestPriceResponse> { }

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
            public double? volume;
            public string volume24h;
            public string prevTotalTurnover;
            public string totalTurnover;
            public string turnover;
            public string turnover24h;
            public string prevPrice24h;
            public double? vwap;
            public double? highPrice;
            public double? lowPrice;
            public decimal? lastPrice;
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
            public double? openValue;
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
