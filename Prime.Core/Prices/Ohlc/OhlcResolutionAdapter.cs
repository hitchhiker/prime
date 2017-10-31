using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Utility;
using System.Linq;
using System.Threading;
using Prime.Common;

namespace Prime.Core
{
    public class OhlcResolutionAdapter
    {
        public OhlcResolutionAdapter(OhlcResolutionAdapterContext context)
        {
            Ctx = context;

            context.EnsureDefaults();

            if (context.RequiresApiDiscovery())
                context.ApplyApiProviders();

            context.EnsureProvider();

            context.Network = context.Network ?? context.PrimaryApiProvider?.Network;

            SeriesId = OhlcUtilities.GetHash(context.Pair, context.TimeResolution, context.Network);

            if (StorageEnabled)
            {
                if (Ctx.MemoryStorageEnabled)
                    _storageAdapters.Add(0, new OhlcResolutionDataAdapterMemory(this));

                if (Ctx.DbStorageEnabled)
                    _storageAdapters.Add(1, new OhlcResolutionDataAdapterDb(this));
            }
            _apiAdapters.Add(0, new OhlcResolutionDataAdapterApi(this));
        }

        private readonly object _lock = new object();
        private readonly object _storageLock = new object();

        public readonly ObjectId SeriesId;

        public readonly OhlcResolutionAdapterContext Ctx;

        public AssetPair Pair => Ctx.Pair;

        public TimeResolution TimeResolution => Ctx.TimeResolution;

        public bool IsDataConverted { get; private set; }

        public bool ApiEnabled => Ctx.ApiEnabled;

        public bool ConversionEnabled => Ctx.ConversionEnabled;

        public bool PeggedEnabled => Ctx.PeggedEnabled;

        public bool StorageEnabled => Ctx.StorageEnabled;

        private readonly Dictionary<int, IOhlcResolutionApi> _apiAdapters = new Dictionary<int, IOhlcResolutionApi>();

        private readonly Dictionary<int, IOhlcResolutionAdapterStorage> _storageAdapters = new Dictionary<int, IOhlcResolutionAdapterStorage>();

        public IReadOnlyList<IOhlcResolutionAdapterStorage> StorageAdapters => _storageAdapters.OrderBy(x => x.Key).Select(x => x.Value).ToList();

        public IReadOnlyList<IOhlcResolutionApi> ApiAdapters => _apiAdapters.OrderBy(x => x.Key).Select(x => x.Value).ToList();

        public ObjectId GetHash(Network network)
        {
            return OhlcUtilities.GetHash(Pair, TimeResolution, network);
        }

        public OhlcData Request(TimeRange timeRange, bool allowLive = false)
        {
            var data = RequestInternal(timeRange, allowLive);
            IsDataConverted = data?.WasConverted == true;
            return data;
        }

        private OhlcData RequestInternal(TimeRange timeRange, bool allowLive = false)
        {
            if (!_apiAdapters.Any() && !_storageAdapters.Any())
                return null;

            if (!StorageEnabled && !ApiEnabled)
                return null;

            lock (_lock)
            {
                OhlcData results = null;

                if (StorageEnabled)
                    results = ContinuousOrMergedStorage(timeRange, allowLive);
                
                var hasRemaining = results.IsEmpty() ? null : results.Remaining(timeRange);

                if (ApiEnabled && (results.IsEmpty() || hasRemaining != null))
                    results = CollectApi(hasRemaining ?? timeRange, results);

                Ctx.Status(results.IsNotEmpty() ? "Data received, processing." : "No data received.");

                if (StorageEnabled && results.IsNotEmpty())
                    StoreResults(timeRange, results);
                
                return results;
            }
        }

        private OhlcData CollectApi(TimeRange range, OhlcData results)
        {
            var apiresults = ApiAdapters.Select(x => x.GetRange(range)).FirstOrDefault(o => o.IsNotEmpty());

            results = results ?? new OhlcData(range.TimeResolution);
            results.Merge(apiresults);
            return results;
        }

        private OhlcData ContinuousOrMergedStorage(TimeRange timeRange, bool allowLive = false)
        {
            var partials = new List<OhlcData>();

            foreach (var r in StorageAdapters.Select(x => x.GetRange(timeRange)))
            {
                if (r.IsEmpty())
                    continue;

                if (!allowLive)
                    r.RemoveAll(x => x.CollectedNearLive);

                if (r.IsCovering(timeRange))
                    return r.HasGap() ? null : r;

                partials.Add(r);
            }

            if (!partials.Any())
                return null;

            var mergedData = new OhlcData(partials.First());
            mergedData.ConvertedFrom = partials.Select(x => x.ConvertedFrom).FirstOrDefault(x=>x!=null) ?? mergedData.ConvertedFrom;

            foreach (var i in partials)
                mergedData.Merge(i);

            if (!timeRange.IsFromInfinity && mergedData.HasGap())
                return null;

            return mergedData;
        }

        private void StoreResults(TimeRange timeRange, OhlcData results)
        {
            var clone = new OhlcData(results); // ienumerable modifications during storage process.

            if (timeRange.TimeResolution != TimeResolution.Day)
                clone.RemoveAll(x => x.DateTimeUtc.IsLive(timeRange.TimeResolution));

            ThreadPool.QueueUserWorkItem(delegate
            {
                lock (_storageLock)
                    Parallel.ForEach(StorageAdapters, a => a.StoreRange(clone, timeRange));
            });
        }
    }
}
