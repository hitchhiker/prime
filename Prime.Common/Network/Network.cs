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

        private IReadOnlyList<IPublicVolumeProvider> _volProviders;
        public IReadOnlyList<IPublicVolumeProvider> PublicVolumeProviders => _volProviders ?? (_volProviders = Providers.OfList<IPublicVolumeProvider>());

        private bool? _hasDirect;
        public bool HasDirect => _hasDirect ?? (bool)(_hasDirect = Providers.Any(x=>x.IsDirect));

        private NetworkData _publicData;
        public NetworkData Data => _publicData ?? (_publicData = NetworkDatas.I.GetOrCreate(PublicContext.I, this));

        public NetworkData DataUsr(UserContext context)
        {
            return NetworkDatas.I.GetOrCreate(context, this);
        }

        private string _code4;
        public string Code4 => _code4 ?? (_code4 = CreateCode4());

        public override string ToString()
        {
            return Name;
        }

        private string CreateCode4()
        {
            var nv = Name.RemoveVowels();
            var c4 = nv.Length > 4 ? nv.ToUpper().Substring(0, 4) : nv.ToUpper();
            if (c4.Length == 4)
                return c4;
            return Name.Length > 4 ? Name.ToUpper().Substring(0, 4) : Name.ToUpper();
        }
    }
}