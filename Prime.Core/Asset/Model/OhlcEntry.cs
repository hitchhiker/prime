using System;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class OhlcEntry : IEquatable<OhlcEntry>
    {
        private OhlcEntry()
        {
        }

        public OhlcEntry(ObjectId seriesId, DateTime utcDateTime, IOhlcProvider provider)
        {
            DateTimeUtc = utcDateTime;
            SeriesId = seriesId;
            Provider = provider;
            Id = (SeriesId + ":" + utcDateTime).GetObjectIdHashCode();
        }

        public OhlcEntry(ObjectId seriesId, DateTime utcDateTime, OhlcResolutionAdapterContext context) : this(seriesId, utcDateTime, context.PrimaryApiProvider)
        {
            ConvertedProvider = context.CurrencyConversionApiProvider;
            ConvertedVia = context.AssetIntermediary;
        }

        public void SetGap(DateTime newDateTimeUtc)
        {
            IsGap = true;
            DateTimeUtc = newDateTimeUtc;
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [Bson]
        public ObjectId SeriesId { get; set; }

        [Bson]
        public Asset ConvertedVia { get; private set; }

        [Bson]
        public bool IsGap { get; private set; }

        [Bson]
        public IOhlcProvider ConvertedProvider { get; private set; }

        [Bson]
        public IOhlcProvider Provider { get; private set; }

        [Bson]
        public DateTime DateTimeUtc { get; set; }

        [Bson]
        public double Open { get; set; }

        [Bson]
        public double Close { get; set; }

        [Bson]
        public double High { get; set; }

        [Bson]
        public double Low { get; set; }

        [Obsolete("Consider type changing to decimal. A lot of volumes are provided in fractionals.")]
        [Bson]
        public long VolumeTo { get; set; }

        [Obsolete("Consider type changing to decimal. A lot of volumes are provided in fractionals.")]
        [Bson]
        public long VolumeFrom { get; set; }

        [Bson]
        public double WeightedAverage { get; set; }

        [Bson]
        public long DateTimeUtcTicks
        {
            get => DateTimeUtc.Ticks;
            set { } //deserialisation only
        }

        [Bson]
        public bool CollectedNearLive { get; set; }

        public bool IsEmpty()
        {
            return Open == 0 && Close == 0 && High == 0 && Low == 0 && WeightedAverage == 00 &&
                   VolumeTo == 0 && VolumeFrom == 0;
        }

        public bool IsPriceEmpty()
        {
            return Open == 0 && Close == 0 && High == 0 && Low == 0;
        }

        public bool Equals(OhlcEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OhlcEntry) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}