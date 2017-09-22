using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Kraken
{
    internal class KrakenSchema
    {
        internal class BaseResponse
        {
            public string[] error;
        }

        internal class BalancesResponse : BaseResponse
        {
            public Dictionary<string, decimal> result;
        }
    }
}
