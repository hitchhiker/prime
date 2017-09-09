using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Utility;
using System.Linq;
using System.Threading;

namespace Prime.Core
{
    public class OhlcResolutionDataAdapter
    {
        public OhlcResolutionDataAdapter(OhlcResolutionAdapterContext context)
        {
            Ctx = context;

            context.EnsureDefaults();

            if (context.RequiresApiDiscovery())
                context.DiscoverAndApplyApiProviders();

            context.EnsureProvider();

            context.Network = context.Network ?? context.PrimaryApiProvider?.Network;

            SeriesId = GetHash(context.Pair, context.TimeResolution, context.Network);

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

        private readonly Dictionary<int, IOhclResolutionApi> _apiAdapters = new Dictionary<int, IOhclResolutionApi>();

        private readonly Dictionary<int, IOhlcResolutionAdapterStorage> _storageAdapters = new Dictionary<int, IOhlcResolutionAdapterStorage>();

        public IReadOnlyList<IOhlcResolutionAdapterStorage> StorageAdapters => _storageAdapters.OrderBy(x => x.Key).Select(x => x.Value).ToList();

        public IReadOnlyList<IOhclResolutionApi> ApiAdapters => _apiAdapters.OrderBy(x => x.Key).Select(x => x.Value).ToList();

        public static ObjectId GetHash(AssetPair pair, TimeResolution market, Network network)
        {
            return $"prime:{pair.Asset1.ShortCode}:{pair.Asset2.ShortCode}:{(int)market}:{network.Id}".GetObjectIdHashCode(true, true);
        }

        public ObjectId GetHash(Network network)
        {
            return GetHash(Pair, TimeResolution, network);
        }

        public OhclData Request(TimeRange timeRange)
        {
            var data = RequestInternal(timeRange);
            IsDataConverted = data?.WasConverted == true;
            return data;
        }

        private OhclData RequestInternal(TimeRange timeRange)
        {
            if (!_apiAdapters.Any() && !_storageAdapters.Any())
                return null;

            if (!StorageEnabled && !ApiEnabled)
                return null;

            lock (_lock)
            {
                OhclData results = null;

                if (StorageEnabled)
                    results = GetOrMergeStorage(timeRange);

                var remaining = results.IsEmpty() ? null : results.RemainingFuture(timeRange);

                if (!results.IsEmpty() && remaining == null)
                    Ctx.Status("Data received, processing.");
                else if (ApiEnabled)
                {
                    var range = remaining ?? timeRange;
                    var apiresults = ApiAdapters.Select(x => x.GetRange(range)).FirstOrDefault(o => o.IsNotEmpty());
                    if (apiresults.IsNotEmpty())
                        Ctx.Status("Data received, processing.");

                    results = results ?? new OhclData(range.TimeResolution);
                    results.Merge(apiresults);
                }

                if (StorageEnabled && results.IsNotEmpty())
                {
                    var clone = new OhclData(results); // ienumerable modifications during storage process.
                    ThreadPool.QueueUserWorkItem(w=> StoreResults(clone, timeRange));
                }

                return results;
            }
        }

        private OhclData GetOrMergeStorage(TimeRange timeRange)
        {
            var partials = new List<OhclData>();

            foreach (var r in StorageAdapters.Select(x => x.GetRange(timeRange)))
            {
                if (r.IsEmpty())
                    continue;

                if (r.IsCovering(timeRange))
                    return r;

                partials.Add(r);
            }

            if (!partials.Any())
                return null;

            var mergedData = new OhclData(partials.First());

            foreach (var i in partials)
                mergedData.Merge(i);

            return mergedData;
        }

        private void StoreResults(OhclData clone, TimeRange timeRange)
        {
            lock (_storageLock)
                Parallel.ForEach(StorageAdapters, a => a.StoreRange(clone, timeRange));
        }
    }
}
