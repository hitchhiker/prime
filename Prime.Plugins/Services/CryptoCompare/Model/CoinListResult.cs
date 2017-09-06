using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins.Services.CryptoCompare.Model
{
    public class CoinListResult : CryptoCompareResponseBase
    {
        public string BaseImageUrl { get; set; }
        public string BaseLinkUrl { get; set; }

        public Dictionary<string, CoinEntry> Data { get; set; }
    }
}
