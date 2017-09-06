using System.Collections.Generic;

namespace CryptoFacilities.Api.V1
{
    internal class GetOpenOrdersResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public List<OrderInfo> Orders { get; set; }
    }
}