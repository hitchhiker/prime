using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Poloniex
{
    public interface IPoloniexApi
    {
        [Post("/tradingApi?command=returnBalances")]
        Task<IEnumerable> GetBalancesAsync([Body] String nonce, [Header("Sign")] String sign);
    }
}
