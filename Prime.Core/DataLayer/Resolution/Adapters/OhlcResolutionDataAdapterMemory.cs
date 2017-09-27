using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class OhlcResolutionDataAdapterMemory : IOhlcResolutionAdapterStorage
    {
        public OhlcResolutionDataAdapterMemory(OhlcResolutionAdapter adapter)
        {
            Ctx = adapter.Ctx;
            _adapter = adapter;
        }

        private static readonly UniqueList<OhlcEntry> MemoryCache = new UniqueList<OhlcEntry>();
        private static readonly UniqueList<CoverageMapMemory> CoverageMapsCache = new UniqueList<CoverageMapMemory>();

        private static readonly object Lock = new object();

        public readonly OhlcResolutionAdapterContext Ctx;
        private readonly OhlcResolutionAdapter _adapter;

        public TimeResolution TimeResolution => _adapter.TimeResolution;

        public OhlcResolutionAdapter Adapter => _adapter;

        public OhlcData GetRange(TimeRange timeRange)
        {
            lock (Lock) { 

                Ctx.Status("Requesting in-memory data");

                var seriesId = _adapter.SeriesId;

                if (!CoverageMap.Covers(timeRange))
                    return null;

                var r = MemoryCache.Where(x => x.SeriesId == seriesId && x.DateTimeUtc >= timeRange.UtcFrom && x.DateTimeUtc <= timeRange.UtcTo).ToList();
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

                if (CoverageMap.Found.Covers(rangeAttempted))
                    return;

                Parallel.ForEach(data, x => x.SeriesId = _adapter.SeriesId);

                MemoryCache.AddRange(data);

                CoverageMap.Include(rangeAttempted, data);
            }
        }

        private CoverageMapMemory _coverageMap;
        public CoverageMapBase CoverageMap => _coverageMap ?? (_coverageMap = Get());

        private CoverageMapMemory Get()
        {
            lock (Lock)
            {
                var cm = CoverageMapsCache.FirstOrDefault(x => x.Id == _adapter.SeriesId);
                if (cm != null)
                    return cm;

                cm = new CoverageMapMemory {Id = _adapter.SeriesId};
                CoverageMapsCache.Add(cm);
                return cm;
            }
        }
    }
}