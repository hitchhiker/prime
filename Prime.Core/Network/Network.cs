using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class Network : IEquatable<Network>, IUniqueIdentifier<ObjectId>
    {
        public Network(string name)
        {
            Name = name;
            NameLowered = name.ToLower();
            _isntEmpty = true;
            Id = ("network:" + NameLowered).GetObjectIdHashCode();
        }

        public readonly ObjectId Id;
        ObjectId IUniqueIdentifier<ObjectId>.Id => Id;

        public string Name { get; private set; }

        public readonly string NameLowered;
        private readonly bool _isntEmpty;
        
        public bool IsEmpty()
        {
            return !_isntEmpty;
        }

        public bool Equals(Network other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var a = obj as Network;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private IReadOnlyList<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers { get { return _providers ?? (_providers = Networks.I.Providers.Where(x=>x.Network.Id == this.Id).ToList());} }
        
        private IReadOnlyList<IExchangeProvider> _eProviders;
        public IReadOnlyList<IExchangeProvider> ExchangeProviders => _eProviders ?? (_eProviders = Providers.OfList<IExchangeProvider>());

        private IReadOnlyList<IWalletService> _wProviders;
        public IReadOnlyList<IWalletService> WalletProviders => _wProviders ?? (_wProviders = Providers.OfList<IWalletService>());

        private IReadOnlyList<IPublicPriceProvider> _ppProviders;
        public IReadOnlyList<IPublicPriceProvider> PublicPriceProviders => _ppProviders ?? (_ppProviders = Providers.OfList<IPublicPriceProvider>());

        private IReadOnlyList<ICoinListProvider> _coinListProviders;
        public IReadOnlyList<ICoinListProvider> CoinListProviders => _coinListProviders ?? (_coinListProviders = Providers.OfList<ICoinListProvider>());

        private IReadOnlyList<IOhlcProvider> _ohlcProviders;
        public IReadOnlyList<IOhlcProvider> OhlcProviders => _ohlcProviders ?? (_ohlcProviders = Providers.OfList<IOhlcProvider>());

        private NetworkData _publicData;
        public NetworkData Data => _publicData ?? (_publicData = NetworkDatas.I.GetOrCreate(PublicContext.I, this));

        public NetworkData DataUsr(UserContext context)
        {
            return NetworkDatas.I.GetOrCreate(context, this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}