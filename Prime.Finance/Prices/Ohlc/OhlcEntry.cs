﻿using System;
using LiteDB;
using Prime.Core;

namespace Prime.Finance
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

        public OhlcEntry(ObjectId seriesId, DateTime utcDateTime, IOhlcProvider primary, IOhlcProvider conversionProvider, Asset intermediary) : this(seriesId, utcDateTime, primary)
        {
            ConvertedProvider = conversionProvider;
            ConvertedVia = intermediary;
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
        public decimal Open { get; set; }

        [Bson]
        public decimal Close { get; set; }

        [Bson]
        public decimal High { get; set; }

        [Bson]
        public decimal Low { get; set; }

        [Bson]
        public decimal VolumeTo { get; set; }

        [Bson]
        public decimal VolumeFrom { get; set; }

        [Bson]
        public decimal WeightedAverage { get; set; }

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