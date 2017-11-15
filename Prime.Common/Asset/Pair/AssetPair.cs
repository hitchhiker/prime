using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPair : IEquatable<AssetPair>, IUniqueIdentifier<ObjectId>
    {
        private AssetPair() { }

        /// <summary>
        /// The first listed currency of a currency pair is called the base currency, and the second currency is called the quote currency.
        /// The currency pair indicates how much of the quote currency is needed to purchase one unit of the base currency.
        /// </summary>
        /// <param name="asset">Base Currency</param>
        /// <param name="quote">Quote Currency</param>
        public AssetPair(Asset asset, Asset quote)
        {
            Asset1 = asset ?? throw new ArgumentException($"{nameof(asset)} is null.");
            Asset2 = quote ?? throw new ArgumentException($"{nameof(quote)} is null.");
        }

        public AssetPair(string asset, string quote) : this(asset.ToAssetRaw(), quote.ToAssetRaw())
        {
        }

        public AssetPair(string asset, string quote, IDescribesAssets provider) : this(Assets.I.Get(asset, provider), Assets.I.Get(quote, provider))
        {
        }

        /// <summary>
        /// Base currency
        /// </summary>
        [Bson]
        public Asset Asset1 { get; private set; }

        /// <summary>
        /// Quote currency. 
        /// </summary>
        [Bson]
        public Asset Asset2 { get; private set; }

        public bool IsEmpty => Equals(Asset1, Asset.None) || Equals(Asset2, Asset.None);

        private bool? _isNormalised;
        public bool IsNormalised => _isNormalised ?? (bool)(_isNormalised = string.CompareOrdinal(Asset1.ShortCode, Asset2.ShortCode) < 0);

        private AssetPair _normalised;
        public AssetPair Normalised => _normalised ?? (_normalised = IsNormalised ? this : Reversed);

        private AssetPair _reversed;
        public AssetPair Reversed => _reversed ?? (_reversed = new AssetPair(Asset2, Asset1));

        public static AssetPair Empty => new AssetPair(Asset.None, Asset.None);

        public bool Has(Asset asset)
        {
            return Asset1.Equals(asset) || Asset2.Equals(asset);
        }

        public Asset Other(Asset asset)
        {
            return Asset1.Equals(asset) ? Asset2 : Asset1;
        }

        public bool EqualsOrReversed(AssetPair pair)
        {
            return this.Equals(pair) || this.Equals(pair.Reversed);
        }

        public bool IsReversed(AssetPair pair)
        {
            return this.Equals(pair.Reversed);
        }

        public string ToTicker(IDescribesAssets converter, string separator = ":")
        {
            return $"{Asset1.ToRemoteCode(converter)}{separator}{Asset2.ToRemoteCode(converter)}";
        }

        [Obsolete]
        public string TickerDash()
        {
            return $"{Asset1.ShortCode}-{Asset2.ShortCode}";
        }

        [Obsolete]
        public string TickerDash(IDescribesAssets converter)
        {
            return $"{Asset1.ToRemoteCode(converter)}-{Asset2.ToRemoteCode(converter)}";
        }

        [Obsolete]
        public string TickerUnderslash()
        {
            return $"{Asset1.ShortCode}_{Asset2.ShortCode}";
        }

        [Obsolete]
        public string TickerUnderslash(IDescribesAssets converter)
        {
            return $"{Asset1.ToRemoteCode(converter)}_{Asset2.ToRemoteCode(converter)}";
        }

        public string TickerSlash(IDescribesAssets converter)
        {
            return $"{Asset1.ToRemoteCode(converter)}/{Asset2.ToRemoteCode(converter)}";
        }

        [Obsolete]
        public string TickerSimple()
        {
            return $"{Asset1.ShortCode}{Asset2.ShortCode}";
        }

        [Obsolete]
        public string TickerSimple(IDescribesAssets converter)
        {
            return $"{Asset1.ToRemoteCode(converter)}{Asset2.ToRemoteCode(converter)}";
        }

        public bool Equals(AssetPair other)
        {
            return Asset1.Equals(other.Asset1) && Asset2.Equals(other.Asset2);
        }

        public bool Equals(AssetPair other, bool eitherDirection)
        {
            if (!eitherDirection)
                return Equals(other);

            return Equals(other) || (Asset1.Equals(other.Asset2) && Asset2.Equals(other.Asset1));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AssetPair && Equals((AssetPair)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Asset1.GetHashCode() * 397) ^ Asset2.GetHashCode();
            }
        }

        public override string ToString()
        {
            return Asset1.ShortCode + ":" + Asset2.ShortCode;
        }

        public static implicit operator AssetPair(string value)
        {
            var p = value.Split(':');
            return p.Length == 2 ? new AssetPair(Assets.I.GetRaw(p[0]), Assets.I.GetRaw(p[1])) : null;
        }

        private UniqueList<AssetPair> _altPairs;
        public IReadOnlyList<AssetPair> PeggedPairs => _altPairs ?? (_altPairs = GetAltPairs());

        private UniqueList<AssetPair> GetAltPairs()
        {
            if (Asset2.Pegged.Count == 0)
                return new UniqueList<AssetPair>();

            var r = new UniqueList<AssetPair>();
            foreach (var i in Asset2.Pegged)
                r.Add(new AssetPair(Asset1, i));

            return r;
        }

        private ObjectId _id;
        public ObjectId Id => _id ?? (_id = ToString().GetObjectIdHashCode());
    }
}