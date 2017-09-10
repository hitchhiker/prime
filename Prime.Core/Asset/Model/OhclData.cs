using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class OhclData : UniqueList<OhclEntry>
    {
        public OhclData(TimeResolution resolution)
        {
            Resolution = resolution;
        }

        public OhclData(TimeResolution resolution, IEnumerable<OhclEntry> entries) : this(resolution)
        {
            base.AddRange(entries);
        }

        public OhclData(OhclData data) : this(data.Resolution, data)
        {
            Resolution = data.Resolution;
            ConvertedFrom = data.ConvertedFrom;
            Network = data.Network;
        }

        public bool IsCovering(TimeRange range)
        {
            return range.UtcFrom >= UtcDataStart && range.UtcTo <= UtcDataEnd;
        }

        public readonly TimeResolution Resolution;
        
        public Asset ConvertedFrom { get; set; }

        public DateTime UtcDataStart => this.MinOrDefault(x => x.DateTimeUtc, DateTime.MinValue);

        public DateTime UtcDataEnd => this.MaxOrDefault(x => x.DateTimeUtc, DateTime.MaxValue);

        public Network Network { get; set; }

        public bool WasConverted => ConvertedFrom != null && !ConvertedFrom.Equals(Asset.None);

        public IReadOnlyList<IOhlcProvider> GetProviders()
        {
            return this.Select(x => x.Provider).DistinctBy(x => x.Id).ToList();
        }
        /// <summary>
        /// Trim's empty data from both ends of this collection/
        /// </summary>
        public OhclData Trim()
        {
            if (Count == 0)
                return this;

            //trim start from empty prices

            var tr = this.OrderBy(x => x.DateTimeUtc).TakeWhile(d => d.IsPriceEmpty()).ToList();
            this.RemoveAll(x => tr.Contains(x));

            tr = this.OrderBy(x => x.DateTimeUtc).TakeWhile(d => d.IsEmpty()).ToList();
            tr.AddRange(this.OrderByDescending(x => x.DateTimeUtc).TakeWhile(d => d.IsEmpty()));
            this.RemoveAll(x => tr.Contains(x));
            return this;
        }

        public bool HasGap()
        {
            if (Count<2)
                return false;

            var lastDate = this.OrderBy(x => x.DateTimeUtc).FirstOrDefault().DateTimeUtc;

            foreach (var i in this.OrderBy(x => x.DateTimeUtc).Skip(1))
            {
                if (lastDate == i.DateTimeUtc || Resolution.Neighbour(lastDate) != i.DateTimeUtc)
                    return true;

                lastDate = i.DateTimeUtc;
            }

            return false;
        }

        public void DeGap()
        {
            if (Count < 2)
                return;

            if (!HasGap())
                return;

            var ord = this.OrderBy(x => x.DateTimeUtc).ToList();

            var previous = ord.FirstOrDefault();
            var bad = new List<OhclEntry>();
            var add = new List<OhclEntry>();

            foreach (var i in ord.Skip(1))
            {
                if (previous.DateTimeUtc == i.DateTimeUtc)
                {
                    bad.Add(i);
                    continue;
                }

                var expected = Resolution.Neighbour(previous.DateTimeUtc);

                if (expected == DateTime.MinValue)
                    throw new Exception("Resolution bad while normalising " + GetType());

                if (expected == i.DateTimeUtc)
                {
                    previous = i;
                    continue;
                }

                var ne = previous.CloneBson();
                ne.SetGap(expected);
                add.Add(ne);
                previous = ne;
            }

            RemoveAll(bad.Contains);
            AddRange(add);
        }

        public void Merge(OhclData data)
        {
            foreach (var i in data)
            {
                RemoveAll(x => x.DateTimeUtc == i.DateTimeUtc);
                Add(i);
            }
        }

        public TimeRange Remaining(TimeRange attemptedRange)
        {
            var future = attemptedRange.UtcTo > UtcDataEnd ? new TimeRange(UtcDataEnd, attemptedRange.UtcTo, attemptedRange.TimeResolution) : null;
            if (future!=null || attemptedRange.IsFromInfinity)
                return future;
            var past = attemptedRange.UtcFrom < UtcDataStart ? new TimeRange(attemptedRange.UtcFrom, UtcDataStart, attemptedRange.TimeResolution) : null;
            return past;
        }

        public TimeRange GetTimeRange(TimeResolution timeResolution)
        {
            return Count == 0 ? TimeRange.Empty : new TimeRange(this.Min(x => x.DateTimeUtc), this.Max(x => x.DateTimeUtc), timeResolution);
        }

        public static OhclData Empty => new OhclData(TimeResolution.None);
    }
}
