using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Core;
using Prime.Finance.Prices.Latest;

namespace Prime.Finance
{
    public class FinanceCommon
    {
        private FinanceCommon()
        {
            Aggregator = Core.Prime.I.StartupMessengers.OfType<Messenger>().FirstOrDefault()?.Aggregator;
        }

        public static FinanceCommon I => Lazy.Value;
        private static readonly Lazy<FinanceCommon> Lazy = new Lazy<FinanceCommon>(()=>new FinanceCommon());

        public IDictionary<Asset, uint> DecimalPlaces = GetDecimals();

        private AssetInfos _assetInfos;
        public AssetInfos AssetInfos => _assetInfos ?? (_assetInfos = AssetInfos.Get());

        internal readonly Aggregator Aggregator;

        private static Dictionary<Asset, uint> GetDecimals()
        {
            var d = new Dictionary<Asset, uint>
            {
                {"USD".ToAssetRaw(), 3},
                {"EUR".ToAssetRaw(), 3},
                {"USDT".ToAssetRaw(), 5},
                {"BTC".ToAssetRaw(), 4},
                {"LTC".ToAssetRaw(), 4}
            };
            return d;
        }

        private static readonly PublicDatas PublicDatas = new PublicDatas();

        private PublicData _pubData;
        public PublicData PubData => _pubData ?? (_pubData = PublicDatas.GetOrCreate(PublicContext.I));
    }
}
