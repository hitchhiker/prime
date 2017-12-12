using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Prime.Utility;
using System.Linq;
using LiteDB;

namespace Prime.Common
{
    public class Asset : IEquatable<Asset>, IFormatProvider, IUniqueIdentifier<ObjectId>
    {
        private Asset(string shortCode)
        {
            ShortCode = shortCode.ToUpper();
            Symbol = "#";
        }
        
        internal static Asset InstanceRaw(string shortCode)
        {
            return new Asset(shortCode);
        }

        public string Name => AssetInfo?.FullName ?? ShortCode;

        public string FullName => AssetInfo!=null ? AssetInfo?.FullName + " (" + ShortCode + ")" : ShortCode;

        [Bson]
        public string Symbol { get; private set; }

        [Bson]
        public string ShortCode { get; private set; }

        public Asset FromCurrentCulture() { return None; }

        public object GetFormat(Type formatType)
        {
            return CultureInfo.InvariantCulture.NumberFormat;
        }
        
        public AssetPair ToPair(Asset asset2)
        {
            return new AssetPair(this, asset2);
        }

        public override string ToString()
        {
            return ShortCode;
        }

        [JsonIgnore]
        private AssetInfo _assetInfo;
        [JsonIgnore]
        public AssetInfo AssetInfo => _assetInfo ?? (_assetInfo = PublicContext.I.AssetInfos.FirstOrDefault(this));

        public bool Equals(Asset other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Asset) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        private UniqueList<Asset> _pegged;
        public IReadOnlyList<Asset> Pegged => _pegged ?? (_pegged = AssetPegged.GetPegged(this));

        public static Asset None = Assets.I.GetRaw("###");

        /// <summary>
        /// Considered a BASE currency for now.
        /// </summary>
        public static Asset Btc = Assets.I.GetRaw("BTC");

        public static Asset Eth = Assets.I.GetRaw("ETH");

        public static Asset Xrp = Assets.I.GetRaw("XRP");

        public static Asset Eur = Assets.I.GetRaw("EUR");

        public static Asset Jpy = Assets.I.GetRaw("JPY");

        public static Asset Usd = Assets.I.GetRaw("USD");

        public static Asset UsdT = Assets.I.GetRaw("USDT");

        public static Asset Krw = Assets.I.GetRaw("KRW");
    
        private int? _popularity;
        public int Popularity => _popularity ?? (int)(_popularity = Assets.I.Popular.ToList().IndexOf(this) + 1);

        public bool IsPopular => Popularity != 0;

        private ObjectId _id;
        public ObjectId Id => _id ?? (_id = ShortCode.GetObjectIdHashCode());
    }
}