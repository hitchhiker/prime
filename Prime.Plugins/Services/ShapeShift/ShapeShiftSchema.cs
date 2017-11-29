using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.ShapeShift
{
    internal class ShapeShiftSchema
    {
        internal class RateResponse
        {
            public decimal rate;
            public decimal limit;
        }

        internal class MarketInfoResponse : RateResponse
        {
            public string pair;
            public decimal min;
            public decimal minerFee;
        }

        internal class MarketInfosResponse : List<MarketInfoResponse>
        {
            
        }
    }
}
