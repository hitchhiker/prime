using System.Collections.Generic;

namespace plugins.Services.BitMex
{
    // Alex - I auto generated this from the API (see the comment below) - the names are wrong, but the structure is correct.

    /*
     * This is the entire BitMex schema
     * http://json2csharp.com/#
     * 
     {
  "Affiliate": {
    "keys": [
      "account",
      "currency"
    ],
    "types": {
      "account": "long",
      "currency": "string",
      "prevPayout": "long",
      "prevTurnover": "long",
      "prevComm": "long",
      "prevTimestamp": "timestamp",
      "execTurnover": "long",
      "execComm": "long",
      "totalReferrals": "long",
      "totalTurnover": "long",
      "totalComm": "long",
      "payoutPcnt": "float",
      "pendingPayout": "long",
      "timestamp": "timestamp"
    }
  },
  "Announcement": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "link": "string",
      "title": "string",
      "content": "string",
      "date": "timestamp"
    }
  },
  "APIKey": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "string",
      "secret": "string",
      "name": "string",
      "nonce": "integer",
      "cidr": "string",
      "permissions": [
        "any"
      ],
      "enabled": "boolean",
      "userId": "integer",
      "created": "timestamp"
    }
  },
  "Broker": {
    "keys": [],
    "types": {}
  },
  "Chat": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "date": "timestamp",
      "user": "string",
      "message": "string",
      "html": "string",
      "fromBot": "boolean"
    }
  },
  "ChatChannel": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "name": "string"
    }
  },
  "ConnectedUsers": {
    "keys": [],
    "types": {
      "users": "integer",
      "bots": "integer"
    }
  },
  "Error": {
    "keys": [
      "error"
    ],
    "types": {
      "error": {
        "message": "string",
        "name": "string"
      }
    }
  },
  "Execution": {
    "keys": [
      "execID"
    ],
    "types": {
      "execID": "guid",
      "orderID": "guid",
      "clOrdID": "string",
      "clOrdLinkID": "string",
      "account": "long",
      "symbol": "string",
      "side": "string",
      "lastQty": "long",
      "lastPx": "float",
      "underlyingLastPx": "float",
      "lastMkt": "string",
      "lastLiquidityInd": "string",
      "simpleOrderQty": "float",
      "orderQty": "long",
      "price": "float",
      "displayQty": "long",
      "stopPx": "float",
      "pegOffsetValue": "float",
      "pegPriceType": "string",
      "currency": "string",
      "settlCurrency": "string",
      "execType": "string",
      "ordType": "string",
      "timeInForce": "string",
      "execInst": "string",
      "contingencyType": "string",
      "exDestination": "string",
      "ordStatus": "string",
      "triggered": "string",
      "workingIndicator": "boolean",
      "ordRejReason": "string",
      "simpleLeavesQty": "float",
      "leavesQty": "long",
      "simpleCumQty": "float",
      "cumQty": "long",
      "avgPx": "float",
      "commission": "float",
      "tradePublishIndicator": "string",
      "multiLegReportingType": "string",
      "text": "string",
      "trdMatchID": "guid",
      "execCost": "long",
      "execComm": "long",
      "homeNotional": "float",
      "foreignNotional": "float",
      "transactTime": "timestamp",
      "timestamp": "timestamp"
    }
  },
  "Funding": {
    "keys": [
      "timestamp",
      "symbol"
    ],
    "types": {
      "timestamp": "timestamp",
      "symbol": "string",
      "fundingInterval": "timespan",
      "fundingRate": "float",
      "fundingRateDaily": "float"
    }
  },
  "IndexComposite": {
    "keys": [
      "timestamp"
    ],
    "types": {
      "timestamp": "timestamp",
      "symbol": "string",
      "indexSymbol": "string",
      "reference": "string",
      "lastPrice": "integer",
      "weight": "integer",
      "logged": "timestamp"
    }
  },
  "Instrument": {
    "keys": [
      "symbol"
    ],
    "types": {
      "symbol": "string",
      "rootSymbol": "string",
      "state": "string",
      "typ": "string",
      "listing": "timestamp",
      "front": "timestamp",
      "expiry": "timestamp",
      "settle": "timestamp",
      "relistInterval": "timespan",
      "inverseLeg": "string",
      "sellLeg": "string",
      "buyLeg": "string",
      "positionCurrency": "string",
      "underlying": "string",
      "quoteCurrency": "string",
      "underlyingSymbol": "string",
      "reference": "string",
      "referenceSymbol": "string",
      "calcInterval": "timespan",
      "publishInterval": "timespan",
      "publishTime": "timespan",
      "maxOrderQty": "long",
      "maxPrice": "float",
      "lotSize": "long",
      "tickSize": "float",
      "multiplier": "long",
      "settlCurrency": "string",
      "underlyingToPositionMultiplier": "long",
      "underlyingToSettleMultiplier": "long",
      "quoteToSettleMultiplier": "long",
      "isQuanto": "boolean",
      "isInverse": "boolean",
      "initMargin": "float",
      "maintMargin": "float",
      "riskLimit": "long",
      "riskStep": "long",
      "limit": "float",
      "capped": "boolean",
      "taxed": "boolean",
      "deleverage": "boolean",
      "makerFee": "float",
      "takerFee": "float",
      "settlementFee": "float",
      "insuranceFee": "float",
      "fundingBaseSymbol": "string",
      "fundingQuoteSymbol": "string",
      "fundingPremiumSymbol": "string",
      "fundingTimestamp": "timestamp",
      "fundingInterval": "timespan",
      "fundingRate": "float",
      "indicativeFundingRate": "float",
      "rebalanceTimestamp": "timestamp",
      "rebalanceInterval": "timespan",
      "openingTimestamp": "timestamp",
      "closingTimestamp": "timestamp",
      "sessionInterval": "timespan",
      "prevClosePrice": "float",
      "limitDownPrice": "float",
      "limitUpPrice": "float",
      "bankruptLimitDownPrice": "float",
      "bankruptLimitUpPrice": "float",
      "prevTotalVolume": "long",
      "totalVolume": "long",
      "volume": "long",
      "volume24h": "long",
      "prevTotalTurnover": "long",
      "totalTurnover": "long",
      "turnover": "long",
      "turnover24h": "long",
      "prevPrice24h": "float",
      "vwap": "float",
      "highPrice": "float",
      "lowPrice": "float",
      "lastPrice": "float",
      "lastPriceProtected": "float",
      "lastTickDirection": "string",
      "lastChangePcnt": "float",
      "bidPrice": "float",
      "midPrice": "float",
      "askPrice": "float",
      "impactBidPrice": "float",
      "impactMidPrice": "float",
      "impactAskPrice": "float",
      "hasLiquidity": "boolean",
      "openInterest": "long",
      "openValue": "long",
      "fairMethod": "string",
      "fairBasisRate": "float",
      "fairBasis": "float",
      "fairPrice": "float",
      "markMethod": "string",
      "markPrice": "float",
      "indicativeTaxRate": "float",
      "indicativeSettlePrice": "float",
      "settledPrice": "float",
      "timestamp": "timestamp"
    }
  },
  "InstrumentInterval": {
    "keys": [],
    "types": {
      "intervals": [
        "string"
      ],
      "symbols": [
        "string"
      ]
    }
  },
  "Insurance": {
    "keys": [
      "currency",
      "timestamp"
    ],
    "types": {
      "currency": "string",
      "timestamp": "timestamp",
      "walletBalance": "long"
    }
  },
  "Leaderboard": {
    "keys": [
      "name"
    ],
    "types": {
      "name": "string",
      "isRealName": "boolean",
      "isMe": "boolean",
      "profit": "integer"
    }
  },
  "Liquidation": {
    "keys": [
      "orderID"
    ],
    "types": {
      "orderID": "guid",
      "symbol": "string",
      "side": "string",
      "price": "float",
      "leavesQty": "long"
    }
  },
  "LiquidationOrder": {
    "keys": [],
    "types": {
      "symbol": "string",
      "side": "string",
      "qty": "integer",
      "price": "integer"
    }
  },
  "LoginRecord": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "date": "timestamp",
      "userId": "integer",
      "ip": "string"
    }
  },
  "Margin": {
    "keys": [
      "account",
      "currency"
    ],
    "types": {
      "account": "long",
      "currency": "string",
      "riskLimit": "long",
      "prevState": "string",
      "state": "string",
      "action": "string",
      "amount": "long",
      "pendingCredit": "long",
      "pendingDebit": "long",
      "confirmedDebit": "long",
      "prevRealisedPnl": "long",
      "prevUnrealisedPnl": "long",
      "grossComm": "long",
      "grossOpenCost": "long",
      "grossOpenPremium": "long",
      "grossExecCost": "long",
      "grossMarkValue": "long",
      "riskValue": "long",
      "taxableMargin": "long",
      "initMargin": "long",
      "maintMargin": "long",
      "sessionMargin": "long",
      "targetExcessMargin": "long",
      "varMargin": "long",
      "realisedPnl": "long",
      "unrealisedPnl": "long",
      "indicativeTax": "long",
      "unrealisedProfit": "long",
      "syntheticMargin": "long",
      "walletBalance": "long",
      "marginBalance": "long",
      "marginBalancePcnt": "float",
      "marginLeverage": "float",
      "marginUsedPcnt": "float",
      "excessMargin": "long",
      "excessMarginPcnt": "float",
      "availableMargin": "long",
      "withdrawableMargin": "long",
      "timestamp": "timestamp",
      "grossLastValue": "long",
      "commission": "float"
    }
  },
  "Notification": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "date": "timestamp",
      "title": "string",
      "body": "string",
      "ttl": "integer",
      "type": "string",
      "closable": "boolean",
      "persist": "boolean",
      "waitForVisibility": "boolean",
      "sound": "string"
    }
  },
  "Order": {
    "keys": [
      "orderID"
    ],
    "types": {
      "orderID": "guid",
      "clOrdID": "string",
      "clOrdLinkID": "string",
      "account": "long",
      "symbol": "string",
      "side": "string",
      "simpleOrderQty": "float",
      "orderQty": "long",
      "price": "float",
      "displayQty": "long",
      "stopPx": "float",
      "pegOffsetValue": "float",
      "pegPriceType": "string",
      "currency": "string",
      "settlCurrency": "string",
      "ordType": "string",
      "timeInForce": "string",
      "execInst": "string",
      "contingencyType": "string",
      "exDestination": "string",
      "ordStatus": "string",
      "triggered": "string",
      "workingIndicator": "boolean",
      "ordRejReason": "string",
      "simpleLeavesQty": "float",
      "leavesQty": "long",
      "simpleCumQty": "float",
      "cumQty": "long",
      "avgPx": "float",
      "multiLegReportingType": "string",
      "text": "string",
      "transactTime": "timestamp",
      "timestamp": "timestamp"
    }
  },
  "OrderBook": {
    "keys": [
      "symbol",
      "level"
    ],
    "types": {
      "symbol": "string",
      "level": "long",
      "bidSize": "long",
      "bidPrice": "float",
      "askPrice": "float",
      "askSize": "long",
      "timestamp": "timestamp"
    }
  },
  "OrderBookL2": {
    "keys": [
      "symbol",
      "id",
      "side"
    ],
    "types": {
      "symbol": "string",
      "id": "long",
      "side": "string",
      "size": "long",
      "price": "float"
    }
  },
  "Position": {
    "keys": [
      "account",
      "symbol",
      "currency"
    ],
    "types": {
      "account": "long",
      "symbol": "string",
      "currency": "string",
      "underlying": "string",
      "quoteCurrency": "string",
      "commission": "float",
      "initMarginReq": "float",
      "maintMarginReq": "float",
      "riskLimit": "long",
      "leverage": "float",
      "crossMargin": "boolean",
      "deleveragePercentile": "float",
      "rebalancedPnl": "long",
      "prevRealisedPnl": "long",
      "prevUnrealisedPnl": "long",
      "prevClosePrice": "float",
      "openingTimestamp": "timestamp",
      "openingQty": "long",
      "openingCost": "long",
      "openingComm": "long",
      "openOrderBuyQty": "long",
      "openOrderBuyCost": "long",
      "openOrderBuyPremium": "long",
      "openOrderSellQty": "long",
      "openOrderSellCost": "long",
      "openOrderSellPremium": "long",
      "execBuyQty": "long",
      "execBuyCost": "long",
      "execSellQty": "long",
      "execSellCost": "long",
      "execQty": "long",
      "execCost": "long",
      "execComm": "long",
      "currentTimestamp": "timestamp",
      "currentQty": "long",
      "currentCost": "long",
      "currentComm": "long",
      "realisedCost": "long",
      "unrealisedCost": "long",
      "grossOpenCost": "long",
      "grossOpenPremium": "long",
      "grossExecCost": "long",
      "isOpen": "boolean",
      "markPrice": "float",
      "markValue": "long",
      "riskValue": "long",
      "homeNotional": "float",
      "foreignNotional": "float",
      "posState": "string",
      "posCost": "long",
      "posCost2": "long",
      "posCross": "long",
      "posInit": "long",
      "posComm": "long",
      "posLoss": "long",
      "posMargin": "long",
      "posMaint": "long",
      "posAllowance": "long",
      "taxableMargin": "long",
      "initMargin": "long",
      "maintMargin": "long",
      "sessionMargin": "long",
      "targetExcessMargin": "long",
      "varMargin": "long",
      "realisedGrossPnl": "long",
      "realisedTax": "long",
      "realisedPnl": "long",
      "unrealisedGrossPnl": "long",
      "longBankrupt": "long",
      "shortBankrupt": "long",
      "taxBase": "long",
      "indicativeTaxRate": "float",
      "indicativeTax": "long",
      "unrealisedTax": "long",
      "unrealisedPnl": "long",
      "unrealisedPnlPcnt": "float",
      "unrealisedRoePcnt": "float",
      "simpleQty": "float",
      "simpleCost": "float",
      "simpleValue": "float",
      "simplePnl": "float",
      "simplePnlPcnt": "float",
      "avgCostPrice": "float",
      "avgEntryPrice": "float",
      "breakEvenPrice": "float",
      "marginCallPrice": "float",
      "liquidationPrice": "float",
      "bankruptPrice": "float",
      "timestamp": "timestamp",
      "lastPrice": "float",
      "lastValue": "long"
    }
  },
  "Quote": {
    "keys": [
      "timestamp",
      "symbol"
    ],
    "types": {
      "timestamp": "timestamp",
      "symbol": "string",
      "bidSize": "long",
      "bidPrice": "float",
      "askPrice": "float",
      "askSize": "long"
    }
  },
  "Role": {
    "keys": [],
    "types": {}
  },
  "Secret": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "key": "string",
      "value": "string",
      "created": "timestamp",
      "ttl": "integer"
    }
  },
  "Settlement": {
    "keys": [
      "timestamp",
      "symbol"
    ],
    "types": {
      "timestamp": "timestamp",
      "symbol": "string",
      "settlementType": "string",
      "settledPrice": "float",
      "bankrupt": "long",
      "taxBase": "long",
      "taxRate": "float"
    }
  },
  "Stats": {
    "keys": [
      "rootSymbol"
    ],
    "types": {
      "rootSymbol": "string",
      "currency": "string",
      "volume24h": "integer",
      "turnover24h": "integer",
      "openInterest": "integer",
      "openValue": "integer"
    }
  },
  "StatsHistory": {
    "keys": [
      "date",
      "rootSymbol"
    ],
    "types": {
      "date": "timestamp",
      "rootSymbol": "string",
      "currency": "string",
      "volume": "integer",
      "turnover": "integer"
    }
  },
  "Trade": {
    "keys": [
      "timestamp",
      "symbol"
    ],
    "types": {
      "timestamp": "timestamp",
      "symbol": "string",
      "side": "string",
      "size": "long",
      "price": "float",
      "tickDirection": "string",
      "trdMatchID": "guid",
      "grossValue": "long",
      "homeNotional": "float",
      "foreignNotional": "float"
    }
  },
  "TradeBin": {
    "keys": [
      "timestamp",
      "symbol"
    ],
    "types": {
      "timestamp": "timestamp",
      "symbol": "string",
      "open": "float",
      "high": "float",
      "low": "float",
      "close": "float",
      "trades": "long",
      "volume": "long",
      "vwap": "float",
      "lastSize": "long",
      "turnover": "long",
      "homeNotional": "float",
      "foreignNotional": "float"
    }
  },
  "Transaction": {
    "keys": [
      "transactID"
    ],
    "types": {
      "transactID": "guid",
      "account": "long",
      "currency": "string",
      "transactType": "string",
      "amount": "long",
      "fee": "long",
      "transactStatus": "string",
      "address": "string",
      "tx": "string",
      "text": "string",
      "transactTime": "timestamp",
      "timestamp": "timestamp"
    }
  },
  "User": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "ownerId": "integer",
      "firstname": "string",
      "lastname": "string",
      "username": "string",
      "email": "string",
      "phone": "string",
      "created": "timestamp",
      "lastUpdated": "timestamp",
      "preferences": "UserPreferences",
      "TFAEnabled": "string",
      "affiliateID": "string",
      "pgpPubKey": "string",
      "country": "string"
    }
  },
  "UserCommission": {
    "keys": [],
    "types": {
      "makerFee": "integer",
      "takerFee": "integer",
      "settlementFee": "integer",
      "maxFee": "integer"
    }
  },
  "UserPreferences": {
    "keys": [],
    "types": {
      "animationsEnabled": "boolean",
      "announcementsLastSeen": "timestamp",
      "chatChannelID": "integer",
      "colorTheme": "string",
      "currency": "string",
      "debug": "boolean",
      "disableEmails": [
        "string"
      ],
      "hideConfirmDialogs": [
        "string"
      ],
      "hideConnectionModal": "boolean",
      "hideFromLeaderboard": "boolean",
      "hideNameFromLeaderboard": "boolean",
      "hideNotifications": [
        "string"
      ],
      "locale": "string",
      "msgsSeen": [
        "string"
      ],
      "orderBookBinning": "object",
      "orderBookType": "string",
      "orderClearImmediate": "boolean",
      "orderControlsPlusMinus": "boolean",
      "sounds": [
        "string"
      ],
      "strictIPCheck": "boolean",
      "strictTimeout": "boolean",
      "tickerGroup": "string",
      "tickerPinned": "boolean",
      "tradeLayout": "string"
    }
  },
  "Wallet": {
    "keys": [
      "account",
      "currency"
    ],
    "types": {
      "account": "long",
      "currency": "string",
      "prevDeposited": "long",
      "prevWithdrawn": "long",
      "prevTransferIn": "long",
      "prevTransferOut": "long",
      "prevAmount": "long",
      "prevTimestamp": "timestamp",
      "deltaDeposited": "long",
      "deltaWithdrawn": "long",
      "deltaTransferIn": "long",
      "deltaTransferOut": "long",
      "deltaAmount": "long",
      "deposited": "long",
      "withdrawn": "long",
      "transferIn": "long",
      "transferOut": "long",
      "amount": "long",
      "pendingCredit": "long",
      "pendingDebit": "long",
      "confirmedDebit": "long",
      "timestamp": "timestamp",
      "addr": "string",
      "script": "string",
      "withdrawalLock": "symbols"
    }
  },
  "Webhook": {
    "keys": [
      "id"
    ],
    "types": {
      "id": "integer",
      "created": "timestamp",
      "userId": "integer",
      "ownerId": "integer",
      "url": "string"
    }
  }
}
     */

    internal class BitMexSchemaGenerated
    {
        public class Types
        {
            public string account;
            public string currency;
            public string prevPayout;
            public string prevTurnover;
            public string prevComm;
            public string prevTimestamp;
            public string execTurnover;
            public string execComm;
            public string totalReferrals;
            public string totalTurnover;
            public string totalComm;
            public string payoutPcnt;
            public string pendingPayout;
            public string timestamp;
        }

        public class Affiliate
        {
            public List<string> keys;
            public Types types;
        }

        public class Types2
        {
            public string id;
            public string link;
            public string title;
            public string content;
            public string date;
        }

        public class Announcement
        {
            public List<string> keys;
            public Types2 types;
        }

        public class Types3
        {
            public string id;
            public string secret;
            public string name;
            public string nonce;
            public string cidr;
            public List<string> permissions;
            public string enabled;
            public string userId;
            public string created;
        }

        public class APIKey
        {
            public List<string> keys;
            public Types3 types;
        }

        public class Types4
        {
        }

        public class Broker
        {
            public List<object> keys;
            public Types4 types;
        }

        public class Types5
        {
            public string id;
            public string date;
            public string user;
            public string message;
            public string html;
            public string fromBot;
        }

        public class Chat
        {
            public List<string> keys;
            public Types5 types;
        }

        public class Types6
        {
            public string id;
            public string name;
        }

        public class ChatChannel
        {
            public List<string> keys;
            public Types6 types;
        }

        public class Types7
        {
            public string users;
            public string bots;
        }

        public class ConnectedUsers
        {
            public List<object> keys;
            public Types7 types;
        }

        public class Error2
        {
            public string message;
            public string name;
        }

        public class Types8
        {
            public Error2 error;
        }

        public class Error
        {
            public List<string> keys;
            public Types8 types;
        }

        public class Types9
        {
            public string execID;
            public string orderID;
            public string clOrdID;
            public string clOrdLinkID;
            public string account;
            public string symbol;
            public string side;
            public string lastQty;
            public string lastPx;
            public string underlyingLastPx;
            public string lastMkt;
            public string lastLiquidityInd;
            public string simpleOrderQty;
            public string orderQty;
            public string price;
            public string displayQty;
            public string stopPx;
            public string pegOffsetValue;
            public string pegPriceType;
            public string currency;
            public string settlCurrency;
            public string execType;
            public string ordType;
            public string timeInForce;
            public string execInst;
            public string contingencyType;
            public string exDestination;
            public string ordStatus;
            public string triggered;
            public string workingIndicator;
            public string ordRejReason;
            public string simpleLeavesQty;
            public string leavesQty;
            public string simpleCumQty;
            public string cumQty;
            public string avgPx;
            public string commission;
            public string tradePublishIndicator;
            public string multiLegReportingType;
            public string text;
            public string trdMatchID;
            public string execCost;
            public string execComm;
            public string homeNotional;
            public string foreignNotional;
            public string transactTime;
            public string timestamp;
        }

        public class Execution
        {
            public List<string> keys;
            public Types9 types;
        }

        public class Types10
        {
            public string timestamp;
            public string symbol;
            public string fundingInterval;
            public string fundingRate;
            public string fundingRateDaily;
        }

        public class Funding
        {
            public List<string> keys;
            public Types10 types;
        }

        public class Types11
        {
            public string timestamp;
            public string symbol;
            public string indexSymbol;
            public string reference;
            public string lastPrice;
            public string weight;
            public string logged;
        }

        public class IndexComposite
        {
            public List<string> keys;
            public Types11 types;
        }

        public class Types12
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
            public string volume;
            public string volume24h;
            public string prevTotalTurnover;
            public string totalTurnover;
            public string turnover;
            public string turnover24h;
            public string prevPrice24h;
            public string vwap;
            public string highPrice;
            public string lowPrice;
            public string lastPrice;
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
            public string openValue;
            public string fairMethod;
            public string fairBasisRate;
            public string fairBasis;
            public string fairPrice;
            public string markMethod;
            public string markPrice;
            public string indicativeTaxRate;
            public string indicativeSettlePrice;
            public string settledPrice;
            public string timestamp;
        }

        public class Instrument
        {
            public List<string> keys;
            public Types12 types;
        }

        public class Types13
        {
            public List<string> intervals;
            public List<string> symbols;
        }

        public class InstrumentInterval
        {
            public List<object> keys;
            public Types13 types;
        }

        public class Types14
        {
            public string currency;
            public string timestamp;
            public string walletBalance;
        }

        public class Insurance
        {
            public List<string> keys;
            public Types14 types;
        }

        public class Types15
        {
            public string name;
            public string isRealName;
            public string isMe;
            public string profit;
        }

        public class Leaderboard
        {
            public List<string> keys;
            public Types15 types;
        }

        public class Types16
        {
            public string orderID;
            public string symbol;
            public string side;
            public string price;
            public string leavesQty;
        }

        public class Liquidation
        {
            public List<string> keys;
            public Types16 types;
        }

        public class Types17
        {
            public string symbol;
            public string side;
            public string qty;
            public string price;
        }

        public class LiquidationOrder
        {
            public List<object> keys;
            public Types17 types;
        }

        public class Types18
        {
            public string id;
            public string date;
            public string userId;
            public string ip;
        }

        public class LoginRecord
        {
            public List<string> keys;
            public Types18 types;
        }

        public class Types19
        {
            public string account;
            public string currency;
            public string riskLimit;
            public string prevState;
            public string state;
            public string action;
            public string amount;
            public string pendingCredit;
            public string pendingDebit;
            public string confirmedDebit;
            public string prevRealisedPnl;
            public string prevUnrealisedPnl;
            public string grossComm;
            public string grossOpenCost;
            public string grossOpenPremium;
            public string grossExecCost;
            public string grossMarkValue;
            public string riskValue;
            public string taxableMargin;
            public string initMargin;
            public string maintMargin;
            public string sessionMargin;
            public string targetExcessMargin;
            public string varMargin;
            public string realisedPnl;
            public string unrealisedPnl;
            public string indicativeTax;
            public string unrealisedProfit;
            public string syntheticMargin;
            public string walletBalance;
            public string marginBalance;
            public string marginBalancePcnt;
            public string marginLeverage;
            public string marginUsedPcnt;
            public string excessMargin;
            public string excessMarginPcnt;
            public string availableMargin;
            public string withdrawableMargin;
            public string timestamp;
            public string grossLastValue;
            public string commission;
        }

        public class Margin
        {
            public List<string> keys;
            public Types19 types;
        }

        public class Types20
        {
            public string id;
            public string date;
            public string title;
            public string body;
            public string ttl;
            public string type;
            public string closable;
            public string persist;
            public string waitForVisibility;
            public string sound;
        }

        public class Notification
        {
            public List<string> keys;
            public Types20 types;
        }

        public class Types21
        {
            public string orderID;
            public string clOrdID;
            public string clOrdLinkID;
            public string account;
            public string symbol;
            public string side;
            public string simpleOrderQty;
            public string orderQty;
            public string price;
            public string displayQty;
            public string stopPx;
            public string pegOffsetValue;
            public string pegPriceType;
            public string currency;
            public string settlCurrency;
            public string ordType;
            public string timeInForce;
            public string execInst;
            public string contingencyType;
            public string exDestination;
            public string ordStatus;
            public string triggered;
            public string workingIndicator;
            public string ordRejReason;
            public string simpleLeavesQty;
            public string leavesQty;
            public string simpleCumQty;
            public string cumQty;
            public string avgPx;
            public string multiLegReportingType;
            public string text;
            public string transactTime;
            public string timestamp;
        }

        public class Order
        {
            public List<string> keys;
            public Types21 types;
        }

        public class Types22
        {
            public string symbol;
            public string level;
            public string bidSize;
            public string bidPrice;
            public string askPrice;
            public string askSize;
            public string timestamp;
        }

        public class OrderBook
        {
            public List<string> keys;
            public Types22 types;
        }

        public class Types23
        {
            public string symbol;
            public string id;
            public string side;
            public string size;
            public string price;
        }

        public class OrderBookL2
        {
            public List<string> keys;
            public Types23 types;
        }

        public class Types24
        {
            public string account;
            public string symbol;
            public string currency;
            public string underlying;
            public string quoteCurrency;
            public string commission;
            public string initMarginReq;
            public string maintMarginReq;
            public string riskLimit;
            public string leverage;
            public string crossMargin;
            public string deleveragePercentile;
            public string rebalancedPnl;
            public string prevRealisedPnl;
            public string prevUnrealisedPnl;
            public string prevClosePrice;
            public string openingTimestamp;
            public string openingQty;
            public string openingCost;
            public string openingComm;
            public string openOrderBuyQty;
            public string openOrderBuyCost;
            public string openOrderBuyPremium;
            public string openOrderSellQty;
            public string openOrderSellCost;
            public string openOrderSellPremium;
            public string execBuyQty;
            public string execBuyCost;
            public string execSellQty;
            public string execSellCost;
            public string execQty;
            public string execCost;
            public string execComm;
            public string currentTimestamp;
            public string currentQty;
            public string currentCost;
            public string currentComm;
            public string realisedCost;
            public string unrealisedCost;
            public string grossOpenCost;
            public string grossOpenPremium;
            public string grossExecCost;
            public string isOpen;
            public string markPrice;
            public string markValue;
            public string riskValue;
            public string homeNotional;
            public string foreignNotional;
            public string posState;
            public string posCost;
            public string posCost2;
            public string posCross;
            public string posInit;
            public string posComm;
            public string posLoss;
            public string posMargin;
            public string posMaint;
            public string posAllowance;
            public string taxableMargin;
            public string initMargin;
            public string maintMargin;
            public string sessionMargin;
            public string targetExcessMargin;
            public string varMargin;
            public string realisedGrossPnl;
            public string realisedTax;
            public string realisedPnl;
            public string unrealisedGrossPnl;
            public string longBankrupt;
            public string shortBankrupt;
            public string taxBase;
            public string indicativeTaxRate;
            public string indicativeTax;
            public string unrealisedTax;
            public string unrealisedPnl;
            public string unrealisedPnlPcnt;
            public string unrealisedRoePcnt;
            public string simpleQty;
            public string simpleCost;
            public string simpleValue;
            public string simplePnl;
            public string simplePnlPcnt;
            public string avgCostPrice;
            public string avgEntryPrice;
            public string breakEvenPrice;
            public string marginCallPrice;
            public string liquidationPrice;
            public string bankruptPrice;
            public string timestamp;
            public string lastPrice;
            public string lastValue;
        }

        public class Position
        {
            public List<string> keys;
            public Types24 types;
        }

        public class Types25
        {
            public string timestamp;
            public string symbol;
            public string bidSize;
            public string bidPrice;
            public string askPrice;
            public string askSize;
        }

        public class Quote
        {
            public List<string> keys;
            public Types25 types;
        }

        public class Types26
        {
        }

        public class Role
        {
            public List<object> keys;
            public Types26 types;
        }

        public class Types27
        {
            public string id;
            public string key;
            public string value;
            public string created;
            public string ttl;
        }

        public class Secret
        {
            public List<string> keys;
            public Types27 types;
        }

        public class Types28
        {
            public string timestamp;
            public string symbol;
            public string settlementType;
            public string settledPrice;
            public string bankrupt;
            public string taxBase;
            public string taxRate;
        }

        public class Settlement
        {
            public List<string> keys;
            public Types28 types;
        }

        public class Types29
        {
            public string rootSymbol;
            public string currency;
            public string volume24h;
            public string turnover24h;
            public string openInterest;
            public string openValue;
        }

        public class Stats
        {
            public List<string> keys;
            public Types29 types;
        }

        public class Types30
        {
            public string date;
            public string rootSymbol;
            public string currency;
            public string volume;
            public string turnover;
        }

        public class StatsHistory
        {
            public List<string> keys;
            public Types30 types;
        }

        public class Types31
        {
            public string timestamp;
            public string symbol;
            public string side;
            public string size;
            public string price;
            public string tickDirection;
            public string trdMatchID;
            public string grossValue;
            public string homeNotional;
            public string foreignNotional;
        }

        public class Trade
        {
            public List<string> keys;
            public Types31 types;
        }

        public class Types32
        {
            public string timestamp;
            public string symbol;
            public string open;
            public string high;
            public string low;
            public string close;
            public string trades;
            public string volume;
            public string vwap;
            public string lastSize;
            public string turnover;
            public string homeNotional;
            public string foreignNotional;
        }

        public class TradeBin
        {
            public List<string> keys;
            public Types32 types;
        }

        public class Types33
        {
            public string transactID;
            public string account;
            public string currency;
            public string transactType;
            public string amount;
            public string fee;
            public string transactStatus;
            public string address;
            public string tx;
            public string text;
            public string transactTime;
            public string timestamp;
        }

        public class Transaction
        {
            public List<string> keys;
            public Types33 types;
        }

        public class Types34
        {
            public string id;
            public string ownerId;
            public string firstname;
            public string lastname;
            public string username;
            public string email;
            public string phone;
            public string created;
            public string lastUpdated;
            public string preferences;
            public string TFAEnabled;
            public string affiliateID;
            public string pgpPubKey;
            public string country;
        }

        public class User
        {
            public List<string> keys;
            public Types34 types;
        }

        public class Types35
        {
            public string makerFee;
            public string takerFee;
            public string settlementFee;
            public string maxFee;
        }

        public class UserCommission
        {
            public List<object> keys;
            public Types35 types;
        }

        public class Types36
        {
            public string animationsEnabled;
            public string announcementsLastSeen;
            public string chatChannelID;
            public string colorTheme;
            public string currency;
            public string debug;
            public List<string> disableEmails;
            public List<string> hideConfirmDialogs;
            public string hideConnectionModal;
            public string hideFromLeaderboard;
            public string hideNameFromLeaderboard;
            public List<string> hideNotifications;
            public string locale;
            public List<string> msgsSeen;
            public string orderBookBinning;
            public string orderBookType;
            public string orderClearImmediate;
            public string orderControlsPlusMinus;
            public List<string> sounds;
            public string strictIPCheck;
            public string strictTimeout;
            public string tickerGroup;
            public string tickerPinned;
            public string tradeLayout;
        }

        public class UserPreferences
        {
            public List<object> keys;
            public Types36 types;
        }

        public class Types37
        {
            public string account;
            public string currency;
            public string prevDeposited;
            public string prevWithdrawn;
            public string prevTransferIn;
            public string prevTransferOut;
            public string prevAmount;
            public string prevTimestamp;
            public string deltaDeposited;
            public string deltaWithdrawn;
            public string deltaTransferIn;
            public string deltaTransferOut;
            public string deltaAmount;
            public string deposited;
            public string withdrawn;
            public string transferIn;
            public string transferOut;
            public string amount;
            public string pendingCredit;
            public string pendingDebit;
            public string confirmedDebit;
            public string timestamp;
            public string addr;
            public string script;
            public string withdrawalLock;
        }

        public class Wallet
        {
            public List<string> keys;
            public Types37 types;
        }

        public class Types38
        {
            public string id;
            public string created;
            public string userId;
            public string ownerId;
            public string url;
        }

        public class Webhook
        {
            public List<string> keys;
            public Types38 types;
        }

        public class RootObject
        {
            public Affiliate Affiliate;
            public Announcement Announcement;
            public APIKey APIKey;
            public Broker Broker;
            public Chat Chat;
            public ChatChannel ChatChannel;
            public ConnectedUsers ConnectedUsers;
            public Error Error;
            public Execution Execution;
            public Funding Funding;
            public IndexComposite IndexComposite;
            public Instrument Instrument;
            public InstrumentInterval InstrumentInterval;
            public Insurance Insurance;
            public Leaderboard Leaderboard;
            public Liquidation Liquidation;
            public LiquidationOrder LiquidationOrder;
            public LoginRecord LoginRecord;
            public Margin Margin;
            public Notification Notification;
            public Order Order;
            public OrderBook OrderBook;
            public OrderBookL2 OrderBookL2;
            public Position Position;
            public Quote Quote;
            public Role Role;
            public Secret Secret;
            public Settlement Settlement;
            public Stats Stats;
            public StatsHistory StatsHistory;
            public Trade Trade;
            public TradeBin TradeBin;
            public Transaction Transaction;
            public User User;
            public UserCommission UserCommission;
            public UserPreferences UserPreferences;
            public Wallet Wallet;
            public Webhook Webhook;
        }
    }
}