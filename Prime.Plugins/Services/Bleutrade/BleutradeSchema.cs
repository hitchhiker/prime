using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bleutrade
{
    internal class BleutradeSchema
    {
        internal class BaseResponse<TResult>
        {
            public bool success;
            public string message;
            public TResult result;

            internal object FirstOrDefault(Func<object, bool> p)
            {
                throw new NotImplementedException();
            }
        }

        internal class MarketEntry
        {
            public bool IsActive;
            public string MarketCurrency;
            public string BaseCurrency;
            public string MarketName;
            public decimal PrevDay;
            public decimal High;
            public decimal Low;
            public decimal Last;
            public decimal Average;
            public decimal Volume;
            public decimal BaseVolume;
            public decimal Bid;
            public decimal Ask;
            public DateTime TimeStamp;
        }

        internal class TickerEntry
        {
            public decimal Last;
            public decimal Bid;
            public decimal Ask;
        }
    }
}
