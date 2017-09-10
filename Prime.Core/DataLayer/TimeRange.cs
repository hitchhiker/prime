using System;
using Prime.Utility;

namespace Prime.Core
{
    public class TimeRange : IEquatable<TimeRange>
    {
        protected TimeRange() { }

        public TimeRange(DateTime from, DateTime to, TimeResolution timeResolution)
        {
            if (from.Kind != DateTimeKind.Utc)
                throw new Exception(nameof(from) + " must be DateTime of DateTimeKind.Utc");

            if (to.Kind != DateTimeKind.Utc)
                throw new Exception(nameof(to) + " must be DateTime of DateTimeKind.Utc");

            if (to < from)
                throw new Exception(nameof(to) + " must represent a date after " + nameof(from) + " to be used in " + GetType());

            TimeResolution = timeResolution;
            UtcFrom = from.ConformToResolution(timeResolution);
            UtcTo = to.ConformToResolution(timeResolution);
        }

        public TimeRange(DateTime endPoint, TimeSpan fromSpan, TimeResolution timeResolution) : this(endPoint.Add(fromSpan), endPoint, timeResolution) { }

        public TimeRange(TimeSpan fromNowSpan, TimeResolution timeResolution) : this(DateTime.UtcNow.Add(fromNowSpan), DateTime.UtcNow, timeResolution) { }

        public int GetDistanceInResolutionTicks()
        {
            var timespan = UtcTo-UtcFrom;
            switch (TimeResolution)
            {
                case Core.TimeResolution.Day:
                    return (int)Math.Ceiling(timespan.TotalDays);
                case Core.TimeResolution.Hour:
                    return (int)Math.Ceiling(timespan.TotalHours);
                case Core.TimeResolution.Minute:
                    return (int)Math.Ceiling(timespan.TotalMinutes);
                default:
                    return 0;
            }
        }

        [Bson]
        public TimeResolution TimeResolution { get; set; }

        [Bson]
        public DateTime UtcFrom { get; private set; }

        [Bson]
        public DateTime UtcTo { get; private set; }

        public bool Equals(TimeRange other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UtcFrom.Equals(other.UtcFrom) && UtcTo.Equals(other.UtcTo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TimeRange) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UtcFrom.GetHashCode() * 397) ^ UtcTo.GetHashCode();
            }
        }

        public static TimeRange EveryDayTillNow => new TimeRange(DateTimeExt.UnixEpoch, DateTime.UtcNow, TimeResolution.Day);

        public static TimeRange Empty => new TimeRange(DateTime.MinValue, DateTime.MinValue, TimeResolution.None);

        public TimeSpan ToTimeSpan()
        {
            return UtcTo-UtcFrom;
        }

        public bool IsFromInfinity => UtcFrom == DateTimeExt.UnixEpoch;
    }
}