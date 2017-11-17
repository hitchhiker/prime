using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public sealed class Network : IEquatable<Network>, IUniqueIdentifier<ObjectId>
    {
        public readonly string NameLowered;
        public readonly ObjectId Id;
        public readonly string Name;

        public Network(string name)
        {
            Name = name.Trim();
            NameLowered = Name.ToLower();
            Id = GetHash(NameLowered);
        }

        ObjectId IUniqueIdentifier<ObjectId>.Id => Id;

        public static ObjectId GetHash(string name)
        {
            return ("network:" + name).GetObjectIdHashCode(true, true);
        }
        
        public bool Equals(Network other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Network a && Equals(a);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private IReadOnlyList<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers { get { return _providers ?? (_providers = Networks.I.Providers.Where(x=>x.Network.Id == this.Id).ToList());} }

        private IReadOnlyList<IDepositProvider> _depositProviders;
        public IReadOnlyList<IDepositProvider> DepositProviders => _depositProviders ?? (_depositProviders = Providers.OfList<IDepositProvider>());

        private IReadOnlyList<IPublicPricingProvider> _ppProviders;
        public IReadOnlyList<IPublicPricingProvider> PublicPriceProviders => _ppProviders ?? (_ppProviders = Providers.OfList<IPublicPricingProvider>());

        private IReadOnlyList<ICoinInformationProvider> _coinListProviders;
        public IReadOnlyList<ICoinInformationProvider> CoinListProviders => _coinListProviders ?? (_coinListProviders = Providers.OfList<ICoinInformationProvider>());

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