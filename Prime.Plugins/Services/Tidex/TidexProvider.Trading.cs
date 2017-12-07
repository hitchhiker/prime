using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Tidex
{
    public partial class TidexProvider : IOrderLimitProvider
    {
        public Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            throw new NotImplementedException();
        }



        public decimal MinimumTradeVolume { get; }
    }
}
