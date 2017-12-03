using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Utility;

namespace Prime.Common
{
    public class WithdrawalProvider
    {
        private WithdrawalProvider()
        {
            ApproximateTransferTimes = GetApproximateTimesDictionary();
        }

        public static WithdrawalProvider I => Lazy.Value;
        private static readonly Lazy<WithdrawalProvider> Lazy = new Lazy<WithdrawalProvider>(()=>new WithdrawalProvider());

        public readonly IReadOnlyDictionary<Asset, int> ApproximateTransferTimes;

        public int GetApproximateTransferTime(Asset asset)
        {
            return ApproximateTransferTimes.ContainsKey(asset) ? ApproximateTransferTimes[asset] : 30;
        }

        private static IReadOnlyDictionary<Asset, int> GetApproximateTimesDictionary()
        {
            var d = new Dictionary<string, int>
            {
                {"BTC", 60},
                {"BCH", 200},
                {"LTC", 30},
                {"DOGE", 20},
                {"NMC", 60},
                {"XRP", 0},
                {"XLM", 0},
                {"ETH", 6},
                {"ETC", 24},
                {"REP", 6},
                {"ICN", 6},
                {"ZEC", 60},
                {"USDT", 60},
                {"DASH", 15},
                {"GNO", 6},
                {"XMR", 30},
                {"MLN", 6},
                {"EOS", 6}
            };
            return d.ToDictionary(x => x.Key.ToAssetRaw(), y => y.Value);
        }
    }
}
