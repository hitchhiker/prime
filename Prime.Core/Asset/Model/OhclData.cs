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

        public void Merge(OhclData data)
        {
            foreach (var i in data)
            {
                RemoveAll(x => x.DateTimeUtc == i.DateTimeUtc);
                Add(i);
            }
        }

        public TimeRange RemainingFuture(TimeRange attemptedRange)
        {
            return attemptedRange.UtcTo > UtcDataEnd ? new TimeRange(UtcDataEnd, attemptedRange.UtcTo, attemptedRange.TimeResolution) : null;
        }

        public TimeRange GetTimeRange(TimeResolution timeResolution)
        {
            return Count == 0 ? TimeRange.Empty : new TimeRange(this.Min(x => x.DateTimeUtc), this.Max(x => x.DateTimeUtc), timeResolution);
        }

        public static OhclData Empty => new OhclData(TimeResolution.None);
    }
}
