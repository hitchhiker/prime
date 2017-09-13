using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins.Services.BitMex
{
    internal class BitMexSchema
    {
        internal class DepositAddress
        {
            public string address;
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
    }
}
