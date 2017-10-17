using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.BitFlyer
{
    internal class BitFlyerSchema
    {
        internal class PricesResponse : List<PriceResponse>
        {
            
        }

        internal class PriceResponse
        {
            public string product_code;
            public string main_currency;
            public string sub_currency;
            public decimal rate;
        }
    }
}
