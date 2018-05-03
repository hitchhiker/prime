using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Bitlish
{
    internal class BitlishSchema
    {
        #region Base

        internal class ErrorResponse
        {
            public string errcode;
            public string msg;
        }
        #endregion

        #region Private

        internal class AuthenticationResponse
        {
            public string email;
            public string ip;
            public int is_guest;
            public string lang;
            public string level;
            public string login;
            public string token;
            public long uid;
        }

        internal class OrderResponse
        {
            public decimal amount;
            public decimal amount_done;
            public decimal amount_free;
            public long created;
            public string dir;
            public decimal fee;
            public decimal holded;
            public string id;
            public string state;
            public string pair_id;
            public decimal price;
            public string type;
            public long user_id;
        }

        internal class TemplateWalletResponse
        {
            public string account;
            public long created;
            public int enabled;
            public string id;
        }

        internal class WithdrawalResponse
        {
            public PaymentResponse[] payments;
        }

        internal class PaymentResponse
        {
            public string amount;
            public string amount_f;
            public long created;
            public string currency;
            public string deposit_wallet_id;
            public string direction;
            public string errcode;
            public string fee;
            public decimal from_amount;
            public string from_currency;
            public string id;
            public string withdraw_wallet_id;
        }
        #endregion

        #region Public 
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class TickerResponse
        {
            public decimal first;
            public decimal last;
            public decimal max;
            public decimal min;
            public decimal prc;
            public decimal sum;
        }

        internal class OrderBookEntryResponse
        {
            public decimal price;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public decimal ask_end;
            public decimal bid_end;
            public long last;
            public string pair_id;
            public OrderBookEntryResponse[] bid;
            public OrderBookEntryResponse[] ask;
        }
        #endregion
    }
}
