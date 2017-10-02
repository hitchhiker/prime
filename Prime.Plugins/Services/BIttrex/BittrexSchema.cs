using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Bittrex
{
    internal class BittrexSchema
    {
        internal class BaseResponse<TResult>
        {
            public bool success;
            public string message;
            public TResult result;
        }

        internal class BalancesResponse : BaseResponse<IList<BalanceResponse>>
        {
            
        }

        internal class BalanceResponse
        {
            public string Currency;
            public decimal Balance;
            public decimal Available;
            public decimal Pending;
            public string CryptoAddress;
            public bool Requested;
            public string Uuid;
        }
    }
}
