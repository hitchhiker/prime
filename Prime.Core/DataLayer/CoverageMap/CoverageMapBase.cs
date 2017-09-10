using System;
using System.Linq;
using LiteDB;

namespace Prime.Core
{
    public abstract class CoverageMapBase : IEquatable<CoverageMapBase>
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [Bson]
        public DateTime UtcEarliestEntry { get; set; } = DateTime.MaxValue;

        [Bson]
        public TimeRanges Found { get; private set; } = new TimeRanges();

        [Bson]
        public TimeRanges Requested { get; private set; } = new TimeRanges();

        [Bson]
        public TimeRanges Unavailable { get; private set; } = new TimeRanges();

        public void Clear()
        {
            UtcEarliestEntry = DateTime.MaxValue;
            Found.Clear();
            Unavailable.Clear();
            Requested.Clear();
        }

        public bool Covers(TimeRange range)
        {
            return Found.Covers(range) || Unavailable.Covers(range) || Requested.Covers(range);
        }

        public virtual OhclData Include(TimeRange rangeAttempted, OhclData data)
        {
            var range  = data.GetTimeRange(rangeAttempted.TimeResolution);

            if (data.IsNotEmpty())
                Found.Add(range);
            else
                Unavailable.Add(rangeAttempted);

            if (!Unavailable.Covers(rangeAttempted) && !Found.Covers(rangeAttempted))
                Requested.Add(rangeAttempted);

            var foundMin = Found.MinimumFrom();
            var missingMin = Unavailable.MinimumFrom();

            var minDate = foundMin < missingMin ? foundMin : missingMin;

            if (minDate != DateTime.MaxValue)
                UtcEarliestEntry = UtcEarliestEntry > minDate ? minDate : UtcEarliestEntry;

            return data;
        }

        public bool Equals(CoverageMapBase other)
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
            return Equals((CoverageMapBase) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}