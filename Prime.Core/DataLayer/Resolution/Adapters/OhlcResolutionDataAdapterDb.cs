using System;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class OhlcResolutionDataAdapterDb : IOhlcResolutionAdapterStorage
    {
        public OhlcResolutionDataAdapterDb(OhlcResolutionAdapter adapter)
        {
            Ctx = adapter.Ctx;
            _adapter = adapter;
        }

        public readonly OhlcResolutionAdapterContext Ctx;
        private readonly OhlcResolutionAdapter _adapter;
        private AssetPair Pair => Ctx.Pair;

        private static readonly object Lock = new object();

        public TimeResolution TimeResolution => _adapter.TimeResolution;

        public OhlcResolutionAdapter Adapter => _adapter;

        public OhlcData GetRange(TimeRange timeRange)
        {
            lock (Lock) { 
                Ctx.Status("Requesting local data @" + Ctx.Network.Name);

                var seriesId = _adapter.SeriesId;

                var r = GetDbCollection().Where(x => x.SeriesId == seriesId && x.DateTimeUtcTicks >= timeRange.UtcFrom.Ticks && x.DateTimeUtcTicks <= timeRange.UtcTo.Ticks).ToList();
                var d = new OhlcData(timeRange.TimeResolution);
                d.AddRange(r);
                return d;
            }
        }

        public void StoreRange(OhlcData data, TimeRange rangeAttempted)
        {
            lock (Lock)
            {
                if (data == null)
                    return;

                var isInDb = CoverageMap.Found.Covers(rangeAttempted);
                if (isInDb)
                    return;

                var seriesId = _adapter.SeriesId;
                var col = PublicContext.I.GetCollection<OhlcEntry>();
                data.ForEach(x => x.SeriesId = seriesId);

                col.Upsert(data);

                CoverageMap.Include(rangeAttempted, data);
            }
        }

        private bool _indexed;
        private LiteQueryable<OhlcEntry> GetDbCollection()
        {
            if (!_indexed)
            {
                var col = PublicContext.I.GetCollection<OhlcEntry>();
                col.EnsureIndex(x => x.DateTimeUtcTicks);
                col.EnsureIndex(x => x.SeriesId);
                _indexed = true;
            }
            return PublicContext.I.As<OhlcEntry>();
        }

        private CoverageMapModel _coverageMap;
        public CoverageMapBase CoverageMap => _coverageMap ?? (_coverageMap = Get());

        private CoverageMapModel Get()
        {
            lock (Lock)
            {
                var cm = PublicContext.I.As<CoverageMapModel>().FirstOrDefault(x => x.Id == _adapter.SeriesId);
                if (cm != null)
                    return cm;

                cm = new CoverageMapModel {Id = _adapter.SeriesId};
                cm.SavePublic();
                return cm;
            }
        }
    }
}