using System.Collections.Generic;

namespace Prime.Plugins.Services.Bithumb
{
    internal class BithumbSchema
    {
        internal class BaseResponse<TData>
        {
            public short status;
            public TData data;
            public string message;
        }

        internal class TickersResponse : BaseResponse<Dictionary<string, object>> { }

        internal class SingleTickerResponse : BaseResponse<TickerResponse> { }

        internal class TickerResponse
        {
            public decimal opening_price;
            public decimal closing_price;
            public decimal min_price;
            public decimal max_price;
            public decimal average_price;
            public decimal units_traded;
            public decimal volume_1day;
            public decimal volume_7day;
            public decimal buy_price;
            public decimal sell_price;
            public long? date;
        }
    }
}