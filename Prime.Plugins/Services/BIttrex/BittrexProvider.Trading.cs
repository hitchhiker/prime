using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Bittrex
{
    public partial class BittrexProvider
    {
        public async Task<object> GetOrderBookAsync(PrivatePairContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetOrderBookAsync(context.Pair.ToTicker(this)).ConfigureAwait(false);

            CheckResponseErrors(r);

            return null;
        }
    }
}
