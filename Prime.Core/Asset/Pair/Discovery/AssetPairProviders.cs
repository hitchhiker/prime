using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairProviders
    {
        private IReadOnlyList<IOhlcProvider> _providers;

        public AssetPair Pair { get; set; }

        public IOhlcProvider Provider { get; set; }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public IReadOnlyList<IOhlcProvider> Providers
        {
            get => _providers;
            set
            {
                _providers = value;
                Provider = _providers.FirstProvider();
            }
        }

        public AssetPairProviders Via { get; set; }
    }
}