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
                    results = StorageAdapters.Select(x => x.GetRange(timeRange)).FirstOrDefault(o => o.IsNotEmpty());

                if (results.IsNotEmpty())
                    Ctx.Status("Data received, processing.");
                else if (ApiEnabled)
                {
                    results = ApiAdapters.Select(x => x.GetRange(timeRange)).FirstOrDefault(o => o.IsNotEmpty());
                    if (results.IsNotEmpty())
                        Ctx.Status("Data received, processing.");
                }

                if (StorageEnabled && results.IsNotEmpty())
                {
                    var clone = new OhclData(results); // ienumerable modifications during storage process.
                    ThreadPool.QueueUserWorkItem(w=> StoreResults(clone, timeRange));
                }

                return results;
            }
        }

        private void StoreResults(OhclData clone, TimeRange timeRange)
        {
            lock (_storageLock)
                Parallel.ForEach(StorageAdapters, a => a.StoreRange(clone, timeRange));
        }
    }
}
