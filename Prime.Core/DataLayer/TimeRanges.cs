using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class TimeRanges : ISerialiseAsObject
    {
        private readonly object _lock = new object();

        [Bson]
        private UniqueList<TimeRange> Items { get; set; } = new UniqueList<TimeRange>();
        
        public bool Covers(TimeRange range)
        {
            lock (_lock)
                return Items.Any(x => x.UtcFrom <= range.UtcFrom && x.UtcTo >= range.UtcTo);
        }

        public void Add(TimeRange range)
        {
            lock (_lock)
            {
                if (Covers(range))
                    return;

                // first remove any ranges this covers

                var covered = Items.Where(x => x.UtcFrom >= range.UtcFrom && x.UtcTo <= range.UtcTo);
                Items.RemoveAll(covered.Contains);

                // before adding, we're going to detect ranges that could widen the new range.
                // we'll then widen and remove the detected (and now covered) ranges.

                var later = new UniqueList<TimeRange>(
                    Items.Where(x => x.UtcFrom >= range.UtcFrom && x.UtcFrom <= range.UtcTo && x.UtcTo > range.UtcTo));
                var earlier = new UniqueList<TimeRange>(
                    Items.Where(x => x.UtcTo >= range.UtcFrom && x.UtcTo <= range.UtcTo && x.UtcFrom < range.UtcFrom));

                if (later.Count == 0 && earlier.Count == 0)
                {
                    Items.Add(range);
                    return;
                }

                var from = range.UtcFrom;
                var to = range.UtcTo;

                if (earlier.Count > 0)
                    from = earlier.Min(x => x.UtcFrom);

                if (later.Count > 0)
                    to = later.Max(x => x.UtcTo);

                Items.Add(new TimeRange(from, to, range.TimeResolution));
                Items.RemoveAll(later.Contains);
                Items.RemoveAll(earlier.Contains);
            }
        }

        public DateTime MinimumFrom()
        {
            lock (_lock)
                return Items.MinOrDefault(x => x.UtcFrom, DateTime.MaxValue);
        }
    }
}