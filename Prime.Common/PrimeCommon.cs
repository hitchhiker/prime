using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Utility;

namespace Prime.Common
{
    public class PrimeCommon
    {
        private PrimeCommon()
        {
            Environment = TypeCatalogue.I.ImplementInstances<IPrimeEnvironment>().FirstOrDefault();
            if (Environment == null)
                throw new Exception($"Cannot find instance of {nameof(IPrimeEnvironment)}. This is probably due to an installation issue.");
        }

        public static PrimeCommon I => Lazy.Value;
        private static readonly Lazy<PrimeCommon> Lazy = new Lazy<PrimeCommon>(()=>new PrimeCommon());

        public readonly IPrimeEnvironment Environment;

        public IDictionary<Asset, uint> DecimalPlaces = GetDecimals();

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

        public IWindowManager GetWindowManager(UserContext context)
        {
            return TypeCatalogue.I.ImplementInstancesWith<IWindowManager>(context).FirstOrDefault();
        }
    }
}
