using System.Collections.Generic;

namespace CryptoFacilities.Api.V1
{
    internal class GetTradesResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public List<TradeInfo> Trades { get; set; }
    }
}